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
  
   var outputs = new Dictionary<string, object?>();
  
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
                }
            },
        
            GlobalSecondaryIndexes =
            {
                new TableGlobalSecondaryIndexArgs
                {
                    Name = "ProductName-index",
                    HashKey = "ProductName",
                    ProjectionType = "ALL"
                },
            }
            });
    var dynamodbPolicy =
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
                            ""{dynamoDbTable.Arn}"",
                            ""{dynamoDbTable.Arn}/index/*""
                        ]
                    }}]
                }}");
    AttachPoliciesToRole(sampleFunctionRole,("DynamoPolicy", dynamodbPolicy));

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
