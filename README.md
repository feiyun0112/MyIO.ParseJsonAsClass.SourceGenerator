使用 Source generator 实现类似于 Visual Studio 菜单“编辑”>"选择性粘贴">"将 JSON 粘贴为类"的功能，将 JSON 字符串转换为 C# 类。

Use the Source generator to implement functionality similar to the Visual Studio menu "Edit" > "Paste Special" > "Paste JSON as Class" to convert JSON strings to C# classes.

## 如何使用 How to use

1. 在项目中安装 NuGet 包 

Install the NuGet package in the project

```powershell   
dotnet add package MyIO.ParseJsonAsClass.SourceGenerator
```

2. 在项目中添加一个 JSON 文件

Add a JSON file in the project

```json
{
  "code": 200,
  "msg": "ok",
  "obj":{"a":1,"subObj":{"a":1}},
  "data": [
    "1","2"
  ],
  "arrar": [
    {"a":1.0},
    {"a":null}
  ]
}
```

3. 在项目中添加一个 C# 文件

Add a C# file in the project

```csharp
using MyIO;
namespace ConsoleApp1
{
    [ParseJsonAsClass("sample.txt")]
    internal partial class Class1
    { 
    }
}
```

`sample.txt` 是上一步中添加的 JSON 文件的名称。

`sample.txt` is the name of the JSON file added in the previous step.

4. 编译项目

Build the project