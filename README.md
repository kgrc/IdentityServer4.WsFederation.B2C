# IdentityServer4.WsFederation.B2C
**Sample** for implementing WS-Federation IdP support for IdentityServer4

## Overview
IdentityServer4 is designed to be extensible with custom protocol endpoints.
This repo leverages the simple implementation of WS-Federation IdP services and integrates this with Azure B2C.
This is useful for connecting SharePoint or older ASP.NET relying parties to IdentityServer.

**This is not supposed to be a generic WS-Federation implementation, but is rather a sample that you can use 
as a starting point to build your own WS-Federation support (or even for inspiration for integrating other custom protocols, which 
are not natively supported by IdentityServer4).**

The following is a brief description of some technical points of interest. Feel free to amend this document if more details are needed.

## .NET Support
The underlying WS-Federation classes used in this repo are only part of the "desktop" .NET Framework and are not included in .NET Core.

## WS-Federation endpoint
The WS-Federation endpoint (metadata, sign-in and out) is implemented via an MVC controller (~/wsfederation).
This controller handles the WS-Federation protocol requests and redirects the user to the login page if needed.

The login page will then use the normal return URL mechanism to redirect back to the WS-Federation endpoint
to create the protocol response.

## B2C endpoint
The B2C end point details are configured in AppSettings.jSON

## Response generation
The `SignInResponseGenerator` class does the heavy lifting of creating the contents of the WS-Federation response:

* it calls the IdentityServer profile service to retrieve the configured claims for the relying party
* it tries to map the standard claim types to WS-* style claim types
* it creates the SAML 1.1/2.0 token
* it creates the RSTR (request security token response)

The outcome of these operations is a `SignInResponseMessage` object which then gets turned into a WS-Federation response and sent back to the relying party.

## Configuration
For most parts, the WS-Federation endpoint can use the standard IdentityServer4 client configuration for relying parties.
But there are also options available for setting WS-Federation specific options.

### Defaults
You can configure global defaults in the `WsFederationOptions` class, e.g.:

* default token type (SAML 1.1 or SAML 2.0)
* default hashing and digest algorithms
* default SAML name identifier format
* default mappings from "short" claim types to WS-* claim types

