﻿using System.Collections.Generic;
using GivingAssistant.Business.Matches.Models;

namespace GivingAssistant.Api.Requests.Matches
{
    public class GetUserOrganisationMatchesListResponse
    {
        public IEnumerable<UserOrganisationMatchListModel> Result { get; set; }
    }
}