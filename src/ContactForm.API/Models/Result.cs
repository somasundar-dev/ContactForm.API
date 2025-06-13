using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactForm.API.Models;

public class Result<T>
    where T : class
{
    public bool IsSuccess { get; set; } = true;
    public T? Value { get; set; }
    public List<string> Errors { get; set; } = new();

    public static Result<T> Success(T value)
    {
        return new Result<T> { IsSuccess = true, Value = value };
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Errors = new List<string> { error },
        };
    }
}
