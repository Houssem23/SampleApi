using System;
using System.IO;
using System.Collections.Generic;

using Pulumi;
using Pulumi.Aws.Lambda;
using Pulumi.Aws.Iam;
using Pulumi.Aws.ApiGatewayV2;
using Pulumi.Aws.DynamoDB.Inputs;
using Pulumi.Aws.DynamoDB;
using Deployment = Pulumi.Deployment;


return await Deployment.RunAsync(() =>
{
   // We need to resolve where the lambda packages are.
    var rootDirectory = new DirectoryInfo(Environment.CurrentDirectory).Parent!;
   // Create an AWS resource (S3 Bucket)
   var outputs = new Dictionary<string, object?>();
   //var bucket = new Bucket("my-bucket");
   //role for lambda function 
   var sampleFunctionRole = CreateLambdaRole("sampleLambdaRole");
   //lambda function 
   var sampleFunction = new Function("sampleFunction", new FunctionArgs
    {
        Runtime = Runtime.Dotnet6,
        MemorySize = 1024,
        //TODO:change this to the right dir
        Code = new FileArchive("/home/runner/work/SampleApi/SampleApi/app.zip"),
        Handler = "WebApi::SampleApi.WebApi.LambdaEntryPoint::FunctionHandlerAsync",
        Role = sampleFunctionRole.Arn,
    });
   //api gateway    
   var gateway = new Api($"sampleGateway", new ApiArgs
    {
        ProtocolType = "HTTP",
        RouteSelectionExpression = "${request.method} ${request.path}",
    });
    //gateway + lambda integration
    var lambdaGatewayIntegration = new Integration("sampleIntegration",
        new IntegrationArgs
        {
            ApiId = gateway.Id,
            IntegrationType = "AWS_PROXY",
            IntegrationMethod = "POST",
            IntegrationUri = sampleFunction.Arn,
            PayloadFormatVersion = "2.0",
            TimeoutMilliseconds = 30000,
        });
    // gateway route
    var gatewayRoute = new Route("sampleRoute", new RouteArgs
    {
        ApiId = gateway.Id,
        RouteKey = "$default",
        Target = lambdaGatewayIntegration.Id.Apply(id => $"integrations/{id}"),
    });
    //gateway stage
    var stagingStage = new Stage("sampleStage", new StageArgs
    {
        ApiId = gateway.Id,
        AutoDeploy = true,
    });
    //gateway permission
    var gatewayPermission = new Permission("sampleLambdaGatewayPermission",
        new PermissionArgs
        {
            Action = "lambda:InvokeFunction",
            Function = sampleFunction.Arn,
            Principal = "apigateway.amazonaws.com",
            SourceArn = Output.Format($"{gateway.ExecutionArn}/*")
        });
    
    var logPolicy =
        Output.Create(@"{
            ""Version"": ""2012-10-17"",
            ""Statement"": [{
                ""Effect"": ""Allow"",
                ""Action"": [
                    ""logs:CreateLogGroup"",
                    ""logs:CreateLogStream"",
                    ""logs:PutLogEvents""
                ],
                ""Resource"": ""arn:aws:logs:*:*:*""
            }]
        }");
   //
   //DynamoDbTable
   var dynamoDbTable = new Table("sampleTable", new TableArgs  {
            
            BillingMode = "PAY_PER_REQUEST",
            HashKey = "Id",
            //RangeKey = "SK",
            StreamEnabled = true,
            StreamViewType = "NEW_IMAGE",
            Attributes =
            {
                new TableAttributeArgs
                {
                    Name = "Id",
                    Type = "N",
                },
                new TableAttributeArgs
                {
                    Name = "ProductName",
                    Type = "S",
                },
                /*
                new TableAttributeArgs
                {
                    Name = "ProductDescription",
                    Type = "S",
                },
                new TableAttributeArgs
                {
                    Name = "Rank",
                    Type = "N",
                },
                */
            },
        
            GlobalSecondaryIndexes =
            {
                new TableGlobalSecondaryIndexArgs
                {
                    Name = "ProductName-index",
                    HashKey = "ProductName",
                    //RangeKey = "GSI1SK",
                    ProjectionType = "ALL"
                },
            }
            });
   // Export the name of the resources
   outputs.Add("sampleLambdaRole",sampleFunctionRole);
   outputs.Add("sampleFunction",sampleFunction);
   outputs.Add("sampleGateway",gateway);
   outputs.Add("sampleTable",dynamoDbTable);
   //TODO: add different other ressource
   return outputs;

});
//Helper method
static Role CreateLambdaRole(string roleName)
{
    var lambdaRole = new Role(roleName, new RoleArgs
    {
        AssumeRolePolicy =
            @"{
                ""Version"": ""2012-10-17"",
                ""Statement"": [
                    {
                        ""Action"": ""sts:AssumeRole"",
                        ""Principal"": {
                            ""Service"": ""lambda.amazonaws.com""
                        },
                        ""Effect"": ""Allow"",
                        ""Sid"": """"
                    }
                ]
            }"
    });
    return lambdaRole;
}

static void AttachPoliciesToRole(Role roleToAttachPoliciesTo, params (string PolicyName, Output<string> Policy)[] policies)
{
    foreach (var policy in policies)
    {
        new RolePolicy(policy.PolicyName, new RolePolicyArgs
        {
            Role = roleToAttachPoliciesTo.Id,
            Policy = policy.Policy
        });
    }
}


/*

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pulumi;
using Pulumi.Aws.ApiGatewayV2;
using Pulumi.Aws.Iam;
using Pulumi.Aws.Lambda;
using Deployment = Pulumi.Deployment;

var tags = new InputMap<string>
{
    {"managedBy", "Pulumi"},
    {"evision:project_code", "P00472/backoffice"},
    {"evision:purpose", "modernization lab"},
    {"evision:repository", "https://github.com/eVisionSoftware/axiom"},
};

return await Deployment.RunAsync(() =>
{
    // There are two places where this is executed from
    //      1. At development time from the csproj directory.
    //      2. At deployment time from the infra directory in the deployment container.
    // We need to resolve where the lambda packages are.
    var rootDirectory = new DirectoryInfo(Environment.CurrentDirectory).Parent!;
    var tempDir = rootDirectory.GetDirectories().SingleOrDefault(d => d.Name == "temp");
    if (tempDir != null)
    {
        rootDirectory = tempDir.GetDirectories("app").Single();
    }
    Console.WriteLine($"Root dir: {rootDirectory.FullName}");

    var outputs = new Dictionary<string, object?>();

    var sampleFunctionRole = CreateLambdaRole("sampleFunctionRole");
    var sampleFunction = new Function("sampleFunction", new FunctionArgs
    {
        Runtime = Runtime.DotnetCore3d1,
        MemorySize = 1024,
        Code = new FileArchive($"{rootDirectory.FullName}/lambda/sample-web.zip"),
        Handler = "Sample.Web::Axiom.Sample.Web.LambdaEntryPoint::FunctionHandlerAsync",
        Role = sampleFunctionRole.Arn,
    });
    
    
    var logPolicy =
        Output.Create(@"{
            ""Version"": ""2012-10-17"",
            ""Statement"": [{
                ""Effect"": ""Allow"",
                ""Action"": [
                    ""logs:CreateLogGroup"",
                    ""logs:CreateLogStream"",
                    ""logs:PutLogEvents""
                ],
                ""Resource"": ""arn:aws:logs:*:*:*""
            }]
        }");
    
    AttachPoliciesToRole(sampleFunctionRole,
        ("sampleFunctionPolicy", logPolicy));
    
    outputs.Add("endpoint", stagingStage.InvokeUrl);
    
    return outputs;
});


  var tableAndIndexAccessPolicy =
                Output.Format($@"{{
                    ""Version"": ""2012-10-17"",
                    ""Statement"": [{{
                        ""Effect"": ""Allow"",
                        ""Action"": [
                            ""dynamodb:DescribeTable"",
                            ""dynamodb:PutItem"",
                            ""dynamodb:UpdateItem"",
                            ""dynamodb:DeleteItem"",
                            ""dynamodb:BatchWriteItem"",
                            ""dynamodb:GetItem"",
                            ""dynamodb:BatchGetItem"",
                            ""dynamodb:Scan"",
                            ""dynamodb:Query"",
                            ""dynamodb:ConditionCheckItem""
                        ],
                        ""Resource"": [
                            ""{organizationsTable.Arn}"",
                            ""{organizationsTable.Arn}/index/*""
                        ]
                    }}]
                }}");


*/