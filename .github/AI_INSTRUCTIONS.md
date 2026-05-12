# AI Assistant Instructions for Chinese Sales System

## Project Overview
This repository contains an improved version of the Chinese Sales System, with:
- **Backend**: ASP.NET Core API in C#
- **Frontend**: Angular application

## Purpose of this guide
The AI should use this file to understand project structure, rules, and how to split work between Controllers and Repositories.

## Main architecture rules
- Keep Controllers thin. Controllers should only handle HTTP requests, authorization, and response formatting.
- Business logic belongs in Services.
- Data access belongs in Repositories.
- Use DTOs for request/response models, not entity models.
- Use AutoMapper profiles when converting between entities and DTOs.

## Important split
- `CONTROLLERS_GUIDE.md` contains detailed guidance for controllers.
- `REPOSITORIES_GUIDE.md` contains detailed guidance for repositories.
These files are separated because Controllers and Repositories are more detailed and should not consume tokens unnecessarily when asking about unrelated topics.

## Key sections to follow
1. Controller conventions
2. Repository conventions
3. Service and DI rules
4. Common tasks and best practices

## Useful notes
- When asked about controllers, first refer to `.github/CONTROLLERS_GUIDE.md`.
- When asked about repositories, first refer to `.github/REPOSITORIES_GUIDE.md`.
- Avoid using tokens to repeat general architecture knowledge.

## When asking the AI
- Use specific requests like: "Add pagination to GiftController", "Create a repository for new entity", "Fix validation in UserDto".
- Avoid broad non-actionable prompts.

---
**Last updated**: May 12, 2026
