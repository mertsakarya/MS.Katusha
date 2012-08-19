using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon.S3.Model;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Services.Configuration;
using MS.Katusha.Services.Configuration.Data;

namespace MS.Katusha.FileSystems
{
    public class S3FileSystem : IKatushaFileSystem
    {
        private readonly BucketData _bucket;

        public S3FileSystem(string bucketName = "")
        {
            _bucket = KatushaConfigurationManager.Instance.GetBucket(bucketName);
        }

        public void Add(string path, Stream stream)
        {
            using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_bucket.AccessKey, _bucket.SecretKey)) {
                if (client == null) return;
                var request = new PutObjectRequest();
                request.WithBucketName(_bucket.BucketName)
                    .WithCannedACL(S3CannedACL.PublicRead)
                    .WithKey(path).InputStream = stream;
                client.PutObject(request);
            }
        }

        //private void EndAdd(IAsyncResult asyncResult)
        //{
        //    if (!asyncResult.IsCompleted) return;
        //    //_notificationService.PhotoOperation(photo, "Added");
        //    var fileToDelete = asyncResult.AsyncState as string;
        //    if (String.IsNullOrWhiteSpace(fileToDelete)) return;
        //}

        //private void EndDelete(IAsyncResult asyncResult)
        //{
        //    if (!asyncResult.IsCompleted) return;
        //    //_notificationService.PhotoOperation(photo, "Deleted");
        //    var fileToDelete = asyncResult.AsyncState as string;
        //    if (String.IsNullOrWhiteSpace(fileToDelete)) return;
        //}

        private void Delete(string path)
        {
            using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_bucket.AccessKey, _bucket.SecretKey)) {
                var request = new DeleteObjectRequest();
                request.WithBucketName(_bucket.BucketName).WithKey(path);
                client.DeleteObject(request);
            }
        }

        private void Delete(params string[] paths)
        {
            using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_bucket.AccessKey, _bucket.SecretKey)) {
                var request = new DeleteObjectsRequest();
                foreach (var key in paths)
                    request.AddKey(key);
                request.WithBucketName(_bucket.BucketName);
                client.DeleteObjects(request);
            }
        }

        public void DeletePhoto(Guid photoGuid)
        {
            var list = new List<string>();
            for (byte i = 0; i <= (byte)PhotoType.MAX; i++) {
                list.Add(GetFileName(photoGuid, i));
            }
            Delete(list.ToArray());
        }

        private static string GetFileName(Guid photoGuid, byte i) { return String.Format("{0}/{1}-{2}.jpg", PhotoFolders.Photos, i, photoGuid); }
        private static string GetBackupFilename(Guid guid) { return String.Format("{0}/{1}.jpg", PhotoFolders.PhotoBackups, guid); }

        public void CopyBackup(Guid guid)
        {
            CopyToBucket(guid, _bucket.BucketName);
        }

        public void CopyToBucket(Guid guid, string bucketName)
        {
            var source = String.Format("{0}/{1}.jpg", PhotoFolders.PhotoBackups, guid);
            var destination = String.Format("{0}/{1}.jpg", PhotoFolders.DeletedPhotos, guid);

            using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_bucket.AccessKey, _bucket.SecretKey)) {
                var request = new CopyObjectRequest {
                    SourceBucket = _bucket.BucketName,
                    SourceKey = source,
                    DestinationBucket = String.IsNullOrWhiteSpace(bucketName) ? _bucket.BucketName : bucketName,
                    DestinationKey = destination
                };
                client.CopyObject(request);
            }
        }

        public void DeleteBackupPhoto(Guid guid)
        {
            CopyBackup(guid);
            Delete(GetBackupFilename(guid));
        }

        public bool FileExists(string path)
        {
            try {
                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_bucket.AccessKey, _bucket.SecretKey)) {
                    client.GetObjectMetadata(new GetObjectMetadataRequest().WithBucketName(_bucket.BucketName).WithKey(path));
                    return true;
                }
            } catch (Amazon.S3.AmazonS3Exception ex) {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return false;

                throw;
            }
        }

        public IList<PhotoFile> GetPhotoNames(out IList<string> unparseableFiles, string prefix = "")
        {
            var list = new List<PhotoFile>();
            using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_bucket.AccessKey, _bucket.SecretKey)) {
                var response = client.ListObjects(new ListObjectsRequest().WithBucketName(_bucket.BucketName).WithPrefix(PhotoFolders.Photos+"/" + prefix));
                unparseableFiles = new List<string>();
                foreach(var s3Object in response.S3Objects) {
                    var fileName = s3Object.Key;
                    var pos = fileName.LastIndexOf('/') + 1;
                    fileName = fileName.Substring(pos);
                    pos = fileName.LastIndexOf(".jpg", StringComparison.Ordinal);
                    if(pos <= 0) unparseableFiles.Add(s3Object.Key);
                    else {
                        if (fileName[1] != '-') unparseableFiles.Add(s3Object.Key);
                        else {
                            try {
                                var type = (byte)(fileName[0] - 48);
                                var guidText = fileName.Substring(2, fileName.Length - 6);
                                Guid guid;
                                if (Guid.TryParse(guidText, out guid)) unparseableFiles.Add(s3Object.Key);
                                var photoFile = new PhotoFile {PhotoType = type, Guid = guid};
                                list.Add(photoFile);
                            } catch {
                                unparseableFiles.Add(fileName);
                            }
                        }
                    }
                }
           }
            return list;
        }

        public byte[] GetData(string path)
        {
            using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_bucket.AccessKey, _bucket.SecretKey)) {
                var request = new GetObjectRequest();
                request.WithBucketName(_bucket.BucketName).WithKey(path);
                using (var response = client.GetObject(request)) {
                    byte[] bytes;
                    using (var memory = new MemoryStream()) {
                        using (var stream = response.ResponseStream) {
                            var data = new byte[32768];
                            int bytesRead;
                            do {
                                bytesRead = stream.Read(data, 0, data.Length);
                                memory.Write(data, 0, bytesRead);
                            } while (bytesRead > 0);
                            memory.Flush();
                            bytes = memory.ToArray();
                        }
                    }
                    return bytes;
                }
            }
        }

        public void ClearPhotos(bool clearBackups = false) {
            IList<string> unparseableFiles;
            var list = GetPhotoNames(out unparseableFiles);
            var files = new List<string>(list.Count + unparseableFiles.Count);
            files.AddRange(unparseableFiles);
            files.AddRange(list.Select(item => GetFileName(item.Guid, item.PhotoType)));
            if(clearBackups) files.AddRange(list.Select(item => GetBackupFilename(item.Guid)));
            Delete(files.ToArray());
        }

        //public string GetString(string path)
        //{
        //    using (var stream = GetStream(path)) {
        //        using (var reader = new StreamReader(stream)) {
        //            return reader.ReadToEnd();
        //        }
        //    }
        //}

        private string GetUrl(string path) { return _bucket.RootUrl + "/" + path; }

        public string GetPhotoUrl(Guid photoGuid, PhotoType photoType, bool encode = false)
        {
            var key = String.Format("{0}/{1}-{2}.jpg", ((photoGuid == Guid.Empty) ? "Images" : "Photos"), (byte) photoType, photoGuid);
            if (!encode) return GetUrl(key);
            try {
                var bytes = GetData(key);
                var base64 = Convert.ToBase64String(bytes);
                return base64;
            } catch {
                return "";
            }
        }

        public void WritePhoto(Photo photo, PhotoType photoType, byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes)) {
                Add(String.Format("{0}/{1}-{2}.jpg", PhotoFolders.Photos, (byte)photoType, photo.Guid), stream);
            } 
        }

        public string GetPhotoBaseUrl() { return _bucket.RootUrl + "/"; }
    }
}
