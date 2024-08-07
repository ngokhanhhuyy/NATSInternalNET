﻿namespace NATSInternal.Services.Dtos;

public class UserListResponseDto
{
    public int PageCount { get; set; }
    public List<UserBasicResponseDto> Results { get; set; }
    public UserListAuthorizationResponseDto Authorization { get; set; }
}