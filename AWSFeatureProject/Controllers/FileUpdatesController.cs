using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AWSFeatureProject.Models;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using AWSFeatureProject.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Amazon.Util;
using Amazon.Runtime;
using Microsoft.AspNetCore.Authorization;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Hosting;


namespace AWSFeatureProject.Controllers
{
    [Authorize]
    public class FileUpdatesController : BaseController
    {

        private const long MaxFileSizeAllowed = 10485760; // 1MB
        private IHostingEnvironment _Env; //manoj
        public FileUpdatesController(IHostingEnvironment envrnmt, MySQLDBContext context)
        {
            _context = context;
            _Env = envrnmt; ;
        }

        // GET: FileUpdates
        public async Task<IActionResult> Index()
        {
           // ClaimsPrincipal cp = new ClaimsPrincipal();
            //cp.Claims.c
            //Get only owne files
            return View(await _context.FileUpdate.Where(s => s.email.Equals(User.Identity.Name)).ToListAsync());
        }

        // GET: FileUpdates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var FileUpdate = await _context.FileUpdate
                .SingleOrDefaultAsync(m => m.Id == id);
            if (FileUpdate == null)
            {
                return NotFound();
            }

            return View(FileUpdate);
        }

        // GET: FileUpdates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FileUpdates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,firstName,lastname,email,upload_date,updated_date,file_name,file_desc")] FileUpdate FileUpdate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(FileUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(FileUpdate);
        }

        // GET: FileUpdates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var FileUpdate = await _context.FileUpdate.SingleOrDefaultAsync(m => m.Id == id);
            if (FileUpdate == null)
            {
                return NotFound();
            }
            return View(FileUpdate);
        }

        // POST: FileUpdates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,firstName,lastname,email,upload_date,updated_date,file_name,file_desc")] FileUpdate FileUpdate)
        {
            if (id != FileUpdate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(FileUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FileUpdateExists(FileUpdate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(FileUpdate);
        }

        // GET: FileUpdates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var FileUpdate = await _context.FileUpdate
                .SingleOrDefaultAsync(m => m.Id == id);
            if (FileUpdate == null)
            {
                return NotFound();
            }

            return View(FileUpdate);
        }

        // POST: FileUpdates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var FileUpdate = await _context.FileUpdate.SingleOrDefaultAsync(m => m.Id == id);
            if (FileUpdate != null)
            {
                _context.FileUpdate.Remove(FileUpdate);
                await _context.SaveChangesAsync();

                //S3 Fil Del
                DeleteObjectRequest delReq = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = FileUpdate.firstName
                };
                AWSCredentials awsCredentials;
                if (_CredentialProfileStoreChain.TryGetAWSCredentials("local-test-profile", out awsCredentials))
                {
                    using (var s3client = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.USWest1))
                    {
                        await s3client.DeleteObjectAsync(delReq);
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FileUpdateExists(int id)
        {
            return _context.FileUpdate.Any(e => e.Id == id);
        }

        public IActionResult ReplaceFile(string fileName)
        {
            FileUpdate o = new FileUpdate();
            o.file_name = fileName;
            return View(o);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplaceFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("No file selected. Please retry with file selection.");
            var filename = Path.GetFileName(file.FileName);

            if (file.Length > MaxFileSizeAllowed)  // 1MB 
            {
                // File exceeds the file maximum size
                ViewBag.Message = "File size is more han allowed limit of " + MaxFileSizeAllowed + " Byte. Please upload smaller file.";
                return RedirectToAction("ReplaceFile", "FileUpdates", new { fileName = filename });
            }

            if (file != null && file.Length > 0)
                try
                {
                    AWSCredentials awsCredentials;
                    if (_CredentialProfileStoreChain.TryGetAWSCredentials("local-test-profile", out awsCredentials))
                    {
                        using (var s3client = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.USWest1))
                        {
                            TransferUtility fileTransferUtility = new TransferUtility(s3client);

                            using (var stream = new MemoryStream())//(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                                fileTransferUtility.Upload(stream, bucketName, filename);
                            }
                        }

                    }
                    //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    //filePath = Path.Combine(filePath, filename);

                    //using (var stream = new FileStream(filePath, FileMode.Create))
                    //{
                    //    await file.CopyToAsync(stream);
                    //}
                    ////AWS
                    //AWSCredentials awsCredentials;
                    //if (_CredentialProfileStoreChain.TryGetAWSCredentials("local-test-profile", out awsCredentials))
                    //{
                    //    using (client = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.USWest1))
                    //    {
                    //        Console.WriteLine("Uploading an object Replace");
                    //        await WritingAnObject(bucketName, filename, filePath);//bucket name, keyName=  key name when object is created, filePath = absolute path to a sample file to upload 
                    //    }
                    //}

                    //System.IO.File.Delete(filePath); //delete file at temp
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }

            return RedirectToAction("ReplaceFile", "FileUpdates", new { fileName = filename });

        }

        //==========File Upload==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadSingle(IFormFile file)
        {

            int recordid = 0;
            //TempData["ErrorMessage"] = "manoj Length="+ file.Length;
            if (file == null || file.Length == 0)
                return RedirectToAction("Create", "FileUpdates", new { fileName = "" });

            if (file.Length > MaxFileSizeAllowed)  // 1MB 
            {
                // File exceeds the file maximum size
                TempData["ErrorMessage"] = "File size is more han allowed limit of  " + MaxFileSizeAllowed + " Byte. Please upload smaller file.";
                return RedirectToAction("Create", "FileUpdates", new { fileName = "" });
            }

            var filename = Path.GetFileName(file.FileName).AppendTimeStamp();
            if (file != null && file.Length > 0)
                try
                {
                    //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TempImages");
                    //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    //filePath = Path.Combine(filePath, filename);
                 
                    AWSCredentials awsCredentials;
                    //_CredentialProfileStoreChain = new CredentialProfileStoreChain(Directory.GetCurrentDirectory() + @"\iam.cred");
                    //var fileContents = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + @"\iam.json");
                    //TempData["ErrorMessage"] = "manoj Length=" + file.Length + ", Before Profile, path=" + Directory.GetCurrentDirectory() + @"\iam.cred" + "env path=" + _Env.WebRootPath+"File Content"+ fileContents;
                    //_CredentialProfileStoreChain = new CredentialProfileStoreChain(_Env.WebRootPath + @"\iam.json");
                    if (_CredentialProfileStoreChain.TryGetAWSCredentials("local-test-profile", out awsCredentials))
                    {
                        //TempData["ErrorMessage"] = "manoj Length=" + file.Length + ", Before Profile, "+" After Profile="+ awsCredentials.ToString();
                        using (var s3client = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.USWest1))
                        {
                            //TransferUtility fileTransferUtility = new TransferUtility(s3client);

                            using (var stream = new MemoryStream())//(filePath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                                long l = stream.Length;
                                //TempData["ErrorMessage"] = "manoj Length=" + file.Length + ", Before Profile, " + " After Profile=" + awsCredentials.ToString()+", Stream length= "+ l.ToString();
                                //fileTransferUtility.Upload(stream, bucketName, filename);
                                //Console.WriteLine("Uploaded an object");
                                PutObjectRequest putRequest2 = new PutObjectRequest
                                {
                                    InputStream = stream,
                                    BucketName = bucketName,
                                    Key = filename,
                                    ContentType = "text/plain"
                                };
                                putRequest2.Metadata.Add("x-amz-meta-title", filename + "ByManoj");
                                var response2 = await s3client.PutObjectAsync(putRequest2);
                                //TempData["ErrorMessage"] = "manoj Length=" + file.Length + ", Before Profile, " + " After Profile=" + awsCredentials.GetCredentials().AccessKey.ToString() + ", Stream Copy= " + l.ToString()+", Response= "+ response2.HttpStatusCode;
                            }
                        }
                    }
                    //AWS

                    // use awsCredentials

                    //var list = ProfileManager.ListProfileNames();
                    //var credentials = new StoredProfileAWSCredentials("local-test-profile");
                    //    using (client = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.USWest1))
                    //    {
                    //        Console.WriteLine("Uploading an object");
                    //        await WritingAnObject(bucketName, filename, filePath);//bucket name, keyName=  key name when object is created, filePath = absolute path to a sample file to upload 
                    //    }
                    //}

                    //System.IO.File.Delete(filePath); //delete file at temp

                    TempData["SuccessMessage"] = "File uploaded successfully";

                    //Save info in DB
                    FileUpdate filedata = new FileUpdate
                    {
                        file_name = filename,
                        firstName = " ",
                        lastname = " ",
                        email = User.Identity.Name,
                        upload_date = DateTime.Now,
                        updated_date = DateTime.Now,
                        file_desc = filename

                    };
                    _context.Add(filedata);
                    int isSuccess = await _context.SaveChangesAsync();
                    if (isSuccess == 1)
                    {
                        recordid = filedata.Id;
                    }
                    else
                        TempData["ErrorMessage"] = "ERROR Occured. Please reupload file.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                TempData["ErrorMessage"] = "You have not specified a file.";
            }
            //https://stackoverflow.com/questions/22505674/can-we-pass-model-as-a-parameter-in-redirecttoaction
            //TempData["filename"] = filename;
            //return RedirectToAction(nameof(Index));
            return RedirectToAction("Edit", "FileUpdates", new { id = recordid });
            //return RedirectToAction("Create", "FileUpdates", new { fileName = filename });
            //return Ok(new { filename  });
        }
        private async Task WritingAnObject(string bucketName, string keyName, string filePath)
        {
            try
            {
                //TransferUtility
                //PutObjectRequest putRequest1 = new PutObjectRequest
                //{

                //    BucketName = bucketName,
                //    Key = keyName,
                //    ContentBody = "sample text"
                //};

                //PutObjectResponse x= await client.PutObjectAsync(putRequest1);
                //response1.RunSynchronously();

                // 2. Put object-set ContentType and add metadata.

                PutObjectRequest putRequest2 = new PutObjectRequest 
                {
                    
                    BucketName = bucketName,
                    Key = keyName,
                    FilePath = filePath,
                    ContentType = "text/plain"
                };
                putRequest2.Metadata.Add("x-amz-meta-title", keyName + "Manoj");

                AWSCredentials awsCredentials;
                if (_CredentialProfileStoreChain.TryGetAWSCredentials("local-test-profile", out awsCredentials))
                {
                    using (var s3client = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.USWest1))
                    {
                        var response2 = await s3client.PutObjectAsync(putRequest2);
                    }
                }
                //response2.RunSynchronously();

            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Check the provided AWS Credentials.");
                    Console.WriteLine(
                        "For service sign up go to http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine(
                        "Error occurred. Message:'{0}' when writing an object"
                        , amazonS3Exception.Message);
                }
            }
        }

    }
}
