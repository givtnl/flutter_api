﻿using NJsonSchema.Annotations;

namespace GivingAssistant.Api.Requests.Questions
{
    public class CreateQuestionResponse
    {
        [NotNull]
        public string Id { get; set; }
    }
}