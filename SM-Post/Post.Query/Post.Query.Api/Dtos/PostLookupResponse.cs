using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Post.Common.Dtos;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Dtos
{
    public class PostLookupResponse : BaseResponse
    {
        public List<PostEntity> Posts { get; set; }
    }
}