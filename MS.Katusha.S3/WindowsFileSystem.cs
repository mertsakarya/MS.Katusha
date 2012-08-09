using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.S3
{
    public class WindowsFileSystem : IKatushaFileSystem
    {
        private readonly string _baseFolderName;

        public WindowsFileSystem(string baseFolderName)
        {
            _baseFolderName = baseFolderName;
        }

        public void Add(string path, Stream stream)
        {
            using (var outStream = new FileStream(_baseFolderName + "\\" + path, FileMode.Create, FileAccess.Write)) {
                var length = (int)stream.Length;
                var bytes = new byte[length];
                stream.Read(bytes, 0, length);
                outStream.Write(bytes, 0, length);
            }
        }

        private static void Delete(string path) { File.Delete(path); }

        public void DeletePhoto(Guid photoGuid, PhotoType photoType)
        {
            Delete(String.Format("{0}/{1}/{2}-{3}.jpg", _baseFolderName, PhotoFolders.Photos, (byte)photoType, photoGuid));
        }

        public void DeleteBackupPhoto(Guid guid)
        {
            Delete(String.Format("{0}/{1}/{2}.jpg", _baseFolderName, PhotoFolders.PhotoBackups, guid));
        }

        public bool FileExists(string path) { return File.Exists(_baseFolderName + "\\" + path); }

        public IList<PhotoFile> GetPhotoNames(out IList<string> unparseableFiles, string prefix = "")
        {
            unparseableFiles = new List<string>();
            var list = new List<PhotoFile>();
            var files = Directory.GetFiles(_baseFolderName + "\\" + PhotoFolders.Photos, prefix + "*.jpg");
            foreach (var fn in files) {
                var pos = fn.LastIndexOf('\\');
                var fileName = fn.Substring(pos);
                pos = fileName.LastIndexOf(".jpg", StringComparison.Ordinal);
                if (pos <= 0) unparseableFiles.Add(fileName);
                else {
                    if (fileName[1] != '-') unparseableFiles.Add(fileName);
                    else {
                        try {
                            var type = (byte)(fileName[0] - 48);
                            var guid = fileName.Substring(2, fileName.Length - 6);
                            var photoFile = new PhotoFile { PhotoType = type, Guid = Guid.Parse(guid) };
                            list.Add(photoFile);
                        } catch {
                            unparseableFiles.Add(fileName);
                        }
                    }
                }
            }
            return list;
        }

        private string GetUrl(string path) { return ConfigurationManager.AppSettings["VirtualPath"] + ((path[0] == '/') ? path.Substring(1) : path); }
        
        public string GetPhotoUrl(Guid photoGuid, PhotoType photoType, bool encode = false) { return GetUrl(String.Format("/{0}/{1}-{2}.jpg", ((photoGuid == Guid.Empty) ? PhotoFolders.Images : PhotoFolders.Photos), (byte)photoType, photoGuid)); }
        public string GetPhotoBaseUrl() { return ConfigurationManager.AppSettings["VirtualPath"]; }

        public void WritePhoto(Photo photo, PhotoType photoType, byte[] bytes)
        {
            var path = String.Format("{0}/{1}/{2}-{3}.jpg", _baseFolderName, PhotoFolders.Photos, (byte) photoType, photo.Guid);
            using(var writer = new FileStream(path, FileMode.Create, FileAccess.Write)) {
                writer.Write(bytes, 0, bytes.Length);
            }
        }

    }
}