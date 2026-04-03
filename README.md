# ASP.NET MVC, Web API, Web Pages, and Razor

## Fork Note:

This repo has the solutions converted to newer formats without changing target versions. Theoretically, these should produce the same outputs as the original [AspNetWebStack](https://github.com/aspnet/AspNetWebStack) repository, however, I cannot guarantee that this is the case.

The following changes made were:
- Replace the `*.sln` files with `*.slnf` files backed by the new `AspNetWebStack.slnx` file.
  - Every file had the text `Runtime.sln` replaced with `Runtime.slnf`. Most of the files were MSBuild files (ie: `*.csproj`, etc), however, there was one `*.cs` file which was looking for the `Runtime.sln`.
  - The new `AspNetWebStack.slnx` includes loose files in the repo in appropriate solution folders.
- Convert all projects to SDK-style projects.
  - As part of this change I manually deleted `<Reference>` items that were pointing to the `packages` as the legacy upgrade assistant did not seem to correctly identify these and delete them.

## Note: This repo is for ASP.NET MVC 5.x, Web API 2.x, and Web Pages 3.x. For ASP.NET Core MVC, check the [AspNetCore repo](https://github.com/aspnet/AspNetCore).

ASP.NET MVC is a web framework that gives you a powerful, patterns-based way to build dynamic websites and Web APIs. ASP.NET MVC enables a clean separation of concerns and gives you full control over markup.

This repo includes:

* ASP.NET MVC 5.x
* ASP.NET Web API 2.x
* ASP.NET Web Pages 3.x
* ASP.NET Razor 3.x

### Contributing

Check out the [contributing](CONTRIBUTING.md) page to see the best places to log issues and start discussions.

### Tags and releases

Git tag or branch|Other products|MVC package versions|Web API package (product) versions|Web Pages package versions
--------|--------------|------------|------------|------------
[v2.0.4](https://github.com/aspnet/AspNetWebStack/tree/v2.0.4)||4.0.40804|4.0.30506|2.0.30506
[v2.1](https://github.com/aspnet/AspNetWebStack/tree/v2.1)|ASP.NET and Web Tools 2012.2, VS 2012 Update 2 (not on http://nuget.org)|v4 2012.2 Update RTM|v1 2012.2 Update RTM|v2 2012.2 Update RTM
[v3.0.2](https://github.com/aspnet/AspNetWebStack/tree/v3.0.2)||5.0.2|5.0.1 (2.0.1)|3.0.1
[v3.1.3](https://github.com/aspnet/AspNetWebStack/tree/v3.1.3)||5.1.3|5.1.2 (2.1.2)|3.1.2
[v3.2.6](https://github.com/aspnet/AspNetWebStack/tree/v3.2.6)||5.2.6|5.2.6|3.2.6
[v3.2.7](https://github.com/aspnet/AspNetWebStack/tree/v3.2.7)||5.2.7|5.2.7|3.2.7
[v3.2.8](https://github.com/aspnet/AspNetWebStack/tree/v3.2.8)||5.2.8|5.2.8|3.2.8
[main](https://github.com/aspnet/AspNetWebStack/tree/main)|New work e.g. MVC 5.2.9-preview1||||
