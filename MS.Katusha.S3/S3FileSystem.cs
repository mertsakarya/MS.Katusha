using System;
using System.Collections.Generic;
using System.IO;
using Amazon.S3.Model;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.S3.Configuration;

namespace MS.Katusha.S3
{
    public class S3FileSystem : IKatushaFileSystem
    {
        private readonly Bucket _bucket;

        public S3FileSystem(string bucketName = "")
        {
            _bucket = S3ConfigurationManager.Instance.GetBucket(bucketName);
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
                list.Add(String.Format("{0}/{1}-{2}.jpg", PhotoFolders.Photos, i, photoGuid));
            }
            Delete(list.ToArray());
        }

        public void DeleteBackupPhoto(Guid guid)
        {
            Delete(String.Format("{0}/{1}.jpg", PhotoFolders.PhotoBackups, guid));
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

        private byte[] GetData(string path)
        {
            using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_bucket.AccessKey, _bucket.SecretKey)) {
                var request = new GetObjectRequest();
                request.WithBucketName(_bucket.BucketName).WithKey(path);
                using (var response = client.GetObject(request)) {
                    var data = new byte[(int)response.ContentLength];
                    response.ResponseStream.Read(data, 0, (int)response.ContentLength);
                    return data;
                }
            }
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
