﻿namespace Api.Models.Comment
{
    public class CommentModel
    {
        public string Author { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTimeOffset Created { get; set; }
    }
}