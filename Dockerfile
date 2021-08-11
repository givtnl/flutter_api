FROM public.ecr.aws/lambda/dotnet:5.0

WORKDIR /var/www/html

COPY ./publish/ ${LAMBDA_TASK_ROOT}

ENV ASPNETCORE_URLS http://*:80

EXPOSE 80

CMD [ "GivingAssistant.Api::GivingAssistant.Api.LambdaFunctionHandler::FunctionHandlerAsync" ]