﻿using LanguageExt;

namespace URLShortener.Application;

public record class CreateShortenedEntryRequest(Option<string> Alias, string Url, DateTime Expiration);
public record class GenerateAliasRequest(int Length, char[] AllowedChars);