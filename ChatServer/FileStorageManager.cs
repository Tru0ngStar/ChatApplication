using System;
using System.Collections.Generic;

namespace ChatServer
{
    public class FileMetadata
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; } // "image", "document", etc.
        public DateTime UploadedTime { get; set; }
        public string Uploader { get; set; }
    }

    public static class FileStorageManager
    {
        private static readonly Dictionary<int, FileMetadata> _files = new();
        private static readonly object _lock = new();
        private static int _nextFileId = 1;

        public static int StoreFile(string fileName, byte[] fileData, string uploader)
        {
            lock (_lock)
            {
                int fileId = _nextFileId++;
                string fileType = GetFileType(fileName);

                var metadata = new FileMetadata
                {
                    FileId = fileId,
                    FileName = fileName,
                    FileData = fileData,
                    FileSize = fileData.Length,
                    FileType = fileType,
                    UploadedTime = DateTime.Now,
                    Uploader = uploader
                };

                _files[fileId] = metadata;
                Console.WriteLine($"[FILE] Lưu file ID {fileId}: {fileName} ({metadata.FileSize} bytes) bởi {uploader}");
                return fileId;
            }
        }

        public static FileMetadata GetFile(int fileId)
        {
            lock (_lock)
            {
                _files.TryGetValue(fileId, out var file);
                return file;
            }
        }

        public static List<ChatShared.FileInfo> GetAllFiles()
        {
            lock (_lock)
            {
                var fileList = new List<ChatShared.FileInfo>();
                foreach (var file in _files.Values)
                {
                    fileList.Add(new ChatShared.FileInfo
                    {
                        Id = file.FileId,
                        FileName = file.FileName,
                        FileSize = file.FileSize,
                        UploadedTime = file.UploadedTime
                    });
                }
                return fileList;
            }
        }

        private static string GetFileType(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();
            return extension switch
            {
                ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".webp" => "image",
                ".pdf" or ".doc" or ".docx" or ".xls" or ".xlsx" => "document",
                ".mp4" or ".avi" or ".mkv" or ".mov" => "video",
                ".mp3" or ".wav" or ".flac" => "audio",
                _ => "file"
            };
        }
    }
}
