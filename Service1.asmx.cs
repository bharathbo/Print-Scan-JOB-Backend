using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Linq;

using System.Web.Script.Serialization; 

namespace vv
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class Service1 : System.Web.Services.WebService
    {
        public Service1() { }

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public void ValidateUser(string username, string password, string roleID)
        {
            HttpContext.Current.Response.ContentType = "application/json";

            try
            {

                var hardcodedUsers = new List<User>
                {
                    new User { Username = "1", Password = "11", RoleID = "1" },
                    new User { Username = "1", Password = "1", RoleID = "1" }
                };


                var user = hardcodedUsers.Find(u =>
                    u.Username == username && u.Password == password && u.RoleID == roleID);

                if (user != null)
                {

                    WriteJsonResponse(new { status = "success", message = "User successfully logged in." });
                }
                else
                {

                    WriteJsonResponse(new { status = "error", message = "Invalid username, password, or role." });
                }
            }
            catch (Exception ex)
            {

                WriteJsonResponse(new { status = "error", message = ex.Message });
            }
        }


        [WebMethod]
        public List<string> GetSystemFolders()
        {
            return new List<string> { "Downloads", "Desktop", "Documents" };
        }


        [WebMethod]
        public List<FileInfo> StartScan(string folderPath)
        {
            var files = GetFilesWithPathsInFolder(folderPath);

            
            string baseFileName = "scanned_pdf";
            string fileExtension = ".pdf";
            int version = 1;
            string fileName = baseFileName + "(" + version + ")" + fileExtension;

           
            while (files.Any(f => f.FileName == fileName))
            {
                version++;
                fileName = baseFileName + "(" + version + ")" + fileExtension;
            }

           
            var newFile = new FileInfo
            {
                FileName = fileName,
                DateModified = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                FileType = "PDF", 
                FileSize = 512    
            };

            // Append the new file to the list of files for the folder
            files.Add(newFile);

            return files;
        }


        [WebMethod]
        public List<FileInfo> GetFilesWithPathsInFolder(string folderPath)
        {
            var files = new Dictionary<string, List<FileInfo>>
{
    { "Downloads", new List<FileInfo> 
        { 
            new FileInfo { FileName = "scan.pdf", DateModified = "2024-12-01", FileType = "PDF", FileSize = 2048 },
            new FileInfo { FileName = "print.pdf", DateModified = "2024-12-01", FileType = "PDF", FileSize = 2048 },
            new FileInfo { FileName = "copy.pdf", DateModified = "2024-12-01", FileType = "PDF", FileSize = 2048 },
            new FileInfo { FileName = "Ifax.pdf", DateModified = "2024-12-01", FileType = "PDF", FileSize = 2048 }
        } 
    },
    { "Desktop", new List<FileInfo> 
        { 
            new FileInfo { FileName = "desktop_print.pdf", DateModified = "2024-12-05", FileType = "PDF", FileSize = 5120 },
            new FileInfo { FileName = "desktop_copy.pdf", DateModified = "2024-12-01", FileType = "PDF", FileSize = 2048 },
            new FileInfo { FileName = "desktop_ifax.pdf", DateModified = "2024-12-01", FileType = "PDF", FileSize = 2048 }
        } 
    },
    { "Documents", new List<FileInfo> 
        { 
            new FileInfo { FileName = "copy.docx", DateModified = "2024-08-03", FileType = "DOCX", FileSize = 4096 },
            new FileInfo { FileName = "document_print.pdf", DateModified = "2024-12-01", FileType = "PDF", FileSize = 2048 },
            new FileInfo { FileName = "document_ifax.pdf", DateModified = "2024-12-01", FileType = "PDF", FileSize = 2048 },
            new FileInfo { FileName = "document_scan.pdf", DateModified = "2024-12-01", FileType = "PDF", FileSize = 2048 }
        } 
    }
};


            return files.ContainsKey(folderPath) ? files[folderPath] : new List<FileInfo>();
        }

        // Method to write the JSON response
        private void WriteJsonResponse(object response)
        {
            var json = new JavaScriptSerializer().Serialize(response); // Serialize response to JSON
            HttpContext.Current.Response.Write(json);
        }

        // User class for validation
        public class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string RoleID { get; set; }
        }

        // FileInfo class for storing file data
        public class FileInfo
        {
            public string FileName { get; set; }
            public string DateModified { get; set; }
            public string FileType { get; set; }
            public int FileSize { get; set; }
        }
    }
}

