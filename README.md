
# SanapBox - AWSFeatureProject
####                                                                                                     By CloudJibe
Personal picture repository in cloud. This application provides secure, highly available and scalable storage for you snaps.
Basically It demonstrates Azure Active Directory Single Sign On Authentication, CRUD operations with AWS RDS MySQL, File storage with AWS S3, File download from AWS CloudFront with AWS SDK for .NET with .NET Core, ASP.NET Core MVC, JQuery, Bootstrap, Pomelo.EntityFrameworkCore.MySql.
#### Main List Page
<img src="images/image_list.PNG">
####  Upload image and Create image metadata Page
<img src="images/create_image.PNG">
<img src="images/create_image_metadata.PNG">
####  Detail Page
<img src="images/detail_image.PNG">
####  Edit Page
<img src="images/rename_image.PNG">
<img src="images/replace_image.PNG">
####  Validations
<img src="images/validationfilesize.PNG">
<img src="images/validationfiletype.PNG">
####  Page
<img src="images/signin.PNG">
<img src="images/signout.PNG">


## Installation
* Visual Studio 2017
* Clone project and open in Visual studio 2017
* Enter your RDS MySQL database connection string in Startup.cs file line # 29.
* Place IAM.json file in toot folder where Startup.cs is there. This file will have you AWS profile information. Sample is as follows.

‘‘‘
[local-test-profile]
aws_access_key_id=XXXXXXXXXXXXXZX
aws_secret_access_key=YYYYYYYYYYYYYYYYYYYYYYYYYYYYYY
region=us-west-1
‘‘‘   
* Now run your application


## License
The SnapBox – By CloudJibe is licensed under the terms of the GPL Open Source license and is available for free.
## Refrence
Manoj Kumar


