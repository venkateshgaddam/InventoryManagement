# InventoryManagement

Hello User,

This is the Backend Project for the Solution of InventoryManagement POC using .net 5.0 c# and WebAPI.

InventoryManagement.Data - Database related Project 
InventoryManagement.Domain - All the business Logic.
InventoryManagement.API - A Cloud hosted Serverless Application which is hosted by AWS CLOUD Serverless Application Model.

# WEBAPI SOLUTION

To run the project please install all the dependencies by running the following command
dotnet restore

once the packegs are installed, run the application 
This is the Conventional WebAPI Solution which consists of 4 Methods for Product Entity

They were : 
1. POST - Add Product - It returns 201 with the Object that has been created - 400/500 incase of Errors
2. PUT - Update Product - It returns 204 - 400/500 incase of a Bad Request
3. GET/{id} - Get the Product info - It returns 200 with the Object Details - 404 if not found
4. DELETE/{id} - Delete the Product Data - Only Soft Delete. Returns 204 If the operation is success.

https://localhost:5001/swagger will direct to the swaggerUI which contains the required input JSON Object with which we can just hit the API with the object


# Authentication

I have used JWT Token Based authentication for this API.
It will be visible in the swagger page. For Good understandability I have kept both Authentication and API controller in the same Project

SO, Landing on the Swagger page will be able to show you the necessary steps for the authentication as well.

I have used Microsoft IDENTITY API for this User Management.

I have created a Developer role and the decorated my Controller with the Authoriza attribute to allow users of only Developer Role (Since it is present in the Question)

Have implemented FluentAPI Validation, so please do look at the ProductValidator Class so that you will get a better understanding of the Validations.








