# Contributing to this repository

## We Develop with Github
We use GitHub to host code, to track issues and feature requests, as well as accept pull requests.

## Report bugs using Github's issues
We use GitHub issues to track bugs. Report a bug by opening a new issue.

## Write bug reports with detail, background, and sample code
Great Bug Reports tend to have:

- A quick summary and/or background
- Steps to reproduce
  - Be specific!
  - Give sample code if you can.
- What you expected would happen
- What actually happens
- Notes (possibly including why you think this might be happening, or what you have tried that didn't work)

## We Use Github Flow, So All Code Changes Happen Through Pull Requests
Pull requests are the best way to propose changes to the codebase. We actively welcome your pull requests:

1. Fork the repo and create your branch from main.
0. If you've added code that should be tested, add tests.
0. Ensure the test suite passes.
0. Create a pull request!

## Working in this repository
### Packages shipped by this repository
These are the packages that are shipped from this repository:
1. Blazor.BrowserExtension
0. Blazor.BrowserExtension.Build
0. Blazor.BrowserExtension.Template

### Blazor.BrowserExtension
This package provides the runtime functionalities, including:
- Importing the browser polyfill
- Importing WebExtensions.Net and registering in dependency container
- Handling the routing in background page and index page

### Blazor.BrowserExtension.Build
This package facilitates the build process by defining and importing the build tasks, including:
- Validating extension manifest
- Replacing content of Blazor framework .js files
- Converting project name to safe string to be used as ID in JavaScript

### Blazor.BrowserExtension.Template
This package serves as a template for initiating a new project using the `dotnet new` command.

## License
By contributing, you agree that your contributions will be licensed under its MIT License.