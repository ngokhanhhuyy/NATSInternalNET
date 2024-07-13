global using System.Reflection;
global using System.Linq.Expressions;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.Text.Json.Serialization;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Storage;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Rendering;
global using Microsoft.AspNetCore.SignalR;
global using MySqlConnector;
global using NATSInternal.Extensions;
global using NATSInternal.Models;
global using NATSInternal.Services;
global using NATSInternal.Services.Constants;
global using NATSInternal.Services.Dtos;
global using NATSInternal.Services.Enums;
global using NATSInternal.Services.Entities;
global using NATSInternal.Services.Exceptions;
global using NATSInternal.Services.Extensions;
global using NATSInternal.Services.Interfaces;
global using NATSInternal.Services.Localization;
global using NATSInternal.Services.Tasks;
global using NATSInternal.Services.Utilities;
global using NATSInternal.Services.Validations;
global using NATSInternal.Services.Validations.Rules;
global using NATSInternal.Services.Validations.Validators;

global using IAuthorizationService = NATSInternal.Services.Interfaces.IAuthorizationService;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using FluentValidation;
global using FluentValidation.Results;
global using ValidationResult = FluentValidation.Results.ValidationResult;
global using ImageMagick;
global using Bogus;
