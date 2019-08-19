# ASP.NET core MVC example with RabbitMQ

```
# start RabbitMQ
docker run -d --hostname my-rabbit --name my-rabbit -p 5671:5671 -p 5672:5672 -p 15672:15672 rabbitmq:3-management

# start application
dotnet run --project .\src\MvcControllerWithRabbitMQ.csproj

# test
1) visit http://localhost:5159/api/todo
2) check console output
```