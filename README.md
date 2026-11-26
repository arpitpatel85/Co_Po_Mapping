# CO-PO Mapping

An ASP.NET Web Forms application for defining Course Outcomes (COs), Program
Outcomes (POs), and mapping COs to POs with correlation levels and approval
workflows.

## Features

- Define and manage Course Outcomes and Program Outcomes.
- Create CO-to-PO mappings and view the mapping matrix.
- Configure correlation levels and attainment percentage ranges.
- Submit mappings for approval and review approval status.

## Project contents

The `Master/` directory contains the Web Forms pages and their C# code-behind
files. This repository snapshot does not include the ASP.NET project file,
database helper (`DbFunctions`), database schema/seed data, or the host
application configuration. The pages depend on those components and are not a
standalone runnable application.

## Requirements

- A compatible ASP.NET Web Forms/.NET Framework host.
- SQL Server and the application database schema.
- The host application's `DbFunctions` implementation and session setup.

## Configuration and secrets

`.env.example` is a placeholder reference for local database settings. The
included application code does **not** read `.env` files; configure the database
through the host application's supported configuration mechanism instead.
Never commit a real `.env`, connection string, password, API key, or private
deployment configuration. `.gitignore` excludes local `.env` files and common
development/build artifacts.

Before publishing, review all files and sample data for credentials, personal
information, institution-specific details, and internal URLs. If a secret was
ever committed or published, treat it as compromised: revoke or rotate it, and
remove it from the repository history as well as the current files.

## Setup

1. Add these pages to the compatible host application.
2. Provide the required database schema and `DbFunctions` implementation.
3. Configure the database through the host application's private configuration.
4. Build and run the host application using its normal deployment process.

The provided files alone are not sufficient to build or run the application.
