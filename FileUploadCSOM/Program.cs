using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploadCSOM
{
    class Program
    {
        static void Main(string[] args)
        {
            UplaodFileTodocumentLibrary();
        }

        private static void UplaodFileTodocumentLibrary()
        {
            string siteUrl = string.Empty;
            string accessToken = string.Empty;

            var sourceFilePath = @"D:\TestDoc.docx";
            var documentLibrary = "Documents";
            string uniqueFileName = Path.GetFileName(sourceFilePath);

            try
            {
                siteUrl = ConfigurationManager.AppSettings["SiteUrl"];
                accessToken = GetToken(siteUrl);
                using (ClientContext context = TokenHelper.GetClientContextWithAccessToken(siteUrl, accessToken))
                {
                    // File object.
                    Microsoft.SharePoint.Client.File uploadFile;

                    List docs = context.Web.Lists.GetByTitle(documentLibrary);

                    using (FileStream fs = new FileStream(sourceFilePath, FileMode.Open))
                    {
                        FileCreationInformation fileInfo = new FileCreationInformation();
                        fileInfo.ContentStream = fs;
                        fileInfo.Url = uniqueFileName;
                        fileInfo.Overwrite = true;

                        string libraryName = "Documents";
                        List library = null;
                        // get the root folder
                        library = context.Web.Lists.GetByTitle(libraryName);
                        context.Load(library, l => l.RootFolder);
                        context.ExecuteQuery();

                        uploadFile = docs.RootFolder.Files.Add(fileInfo);
                        context.Load(uploadFile);
                        context.ExecuteQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private static string GetToken(string siteUrl)
        {
            string accessToken = string.Empty;
            try
            {
                Uri url = new Uri(siteUrl);
                string realm = TokenHelper.GetRealmFromTargetUrl(url);
                accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, url.Authority, realm).AccessToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return accessToken;
        }
    }
}
