using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AWSFeatureProject.Extensions;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using AWSFeatureProject.Models;
using Amazon.Runtime.CredentialManagement;
using Amazon;
using System.IO;
using Amazon.S3;
using Amazon.Runtime;

namespace AWSFeatureProject.Controllers
{
    public class BaseController : Controller
    {
        protected ControllerHelper _helper = null;
       // protected IHostingEnvironment _environment;
        protected MySQLDBContext _context;
        
        protected readonly string bucketName = "homework2-manoj";
        protected CredentialProfileStoreChain _CredentialProfileStoreChain = null;
        public BaseController()
        {
            _helper = new ControllerHelper();
            _CredentialProfileStoreChain = new CredentialProfileStoreChain(Directory.GetCurrentDirectory()+@"\iam.json");

            //AWSCredentials awsCredentials = null;
            //if (_CredentialProfileStoreChain.TryGetAWSCredentials("local-test-profile", out awsCredentials))
            //{
            //    _s3client = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.USWest1);
            //}

                //var options = new CredentialProfileOptions
                //{
                //    AccessKey = "aa",
                //    SecretKey = "aaaa"
                //};
                //var profile = new CredentialProfile("local-test-profile", options);
                ////profile.Region = RegionEndpoint.USWest1;
                //var netSDKFile = new SharedCredentialsFile();
                //netSDKFile.RegisterProfile(profile);
            }

        protected ControllerHelper ControllerHelperClass
        {
            get { return _helper; }
        }

        //protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        //{
        //    string user = User.Identity.Name;

        //    return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
        //}
        
        

    }
}