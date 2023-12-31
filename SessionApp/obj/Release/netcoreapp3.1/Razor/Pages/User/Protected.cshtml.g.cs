#pragma checksum "D:\Universitate\Praktika\Praktika\Learning\C#\SessionApp\Pages\User\Protected.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "e4096973c37e5b2c4b47f108704da9c3cd9d2005"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(SessionApp.Pages.User.Pages_User_Protected), @"mvc.1.0.razor-page", @"/Pages/User/Protected.cshtml")]
namespace SessionApp.Pages.User
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\Universitate\Praktika\Praktika\Learning\C#\SessionApp\Pages\_ViewImports.cshtml"
using SessionApp;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e4096973c37e5b2c4b47f108704da9c3cd9d2005", @"/Pages/User/Protected.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a29bf66a3964600865bdb9070f1fbbb5697a4f82", @"/Pages/_ViewImports.cshtml")]
    #nullable restore
    public class Pages_User_Protected : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    #nullable disable
    {
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("<!DOCTYPE html>\r\n<html>\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral(@"
<script src=""https://ajax.googleapis.com/ajax/libs/jquery/3.6.4/jquery.min.js""></script>
<script src=""https://cdnjs.cloudflare.com/ajax/libs/jquery-cookie/1.4.1/jquery.cookie.min.js""></script>
<script src=""https://cdn.jsdelivr.net/npm/js-cookie@3.0.5/dist/js.cookie.min.js""></script>
<script>
function getCookie(cookieName) {
    var name = cookieName + ""="";
    var decodedCookie = decodeURIComponent(document.cookie);
    var cookieArray = decodedCookie.split(';');

    for (var i = 0; i < cookieArray.length; i++) {
        var cookie = cookieArray[i];
        while (cookie.charAt(0) === ' ') {
            cookie = cookie.substring(1);
        }
        if (cookie.indexOf(name) === 0) {
            return cookie.substring(name.length, cookie.length);
        }
    }
    return """";
}

$(document).ready(function() {
    var sessionId = """";
    //sessionId = getCookie(""SessionKeySessionId"");
    //var urlParams = new URLSearchParams(window.location.search);
    console.log(sessionId);
 ");
                WriteLiteral(@"   //if(urlParams.has('SessionId'))
    //{
    //    sessionId = urlParams.get('SessionId');
    //    //document.cookie = ""SessionKeySessionId=""+sessionId; //set cookie!!
    //    console.log(getCookie(""SessionKeySessionId""));
    //}
    $.ajax({
        url: ""http://alpha.jwtappapi.com/api/authorize/sessionId"",
        type: ""POST"",
        data: JSON.stringify(sessionId),
        contentType: ""application/json; charset=utf-8"",
        xhrFields: {
            withCredentials: true // Include cookies
        },       
        success: function(data){
            console.log(""Success"");
        },
        error: function ajaxError(jqXHR, textStatus, errorThrown) {
            window.location.href = ""http://alpha.jwtappapi.com/api/login?referringUrl=https://localhost:5001/api/user/protected""; // referringUrl - this is where you want to get
        }
    });
    $('#logoutButton').click(function(){
        console.log(""Logging out"");
        $.ajax({
            url:""http://alpha.jwt");
                WriteLiteral(@"appapi.com/api/logout"",
            type: ""GET"",
            xhrFields: {
                withCredentials: true // Include cookies
            },
            success: function()
            {
                console.log(""Logged out"");
                location.reload();
            },
            error: function ajaxError(jqXHR, textStatus, errorThrown) {
            alert(""Error!"");    
            }
        });
    });
});
</script>
");
            }
            );
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "e4096973c37e5b2c4b47f108704da9c3cd9d20055730", async() => {
                WriteLiteral("\r\n    <p>Protected page!!!</p>\r\n    <br />\r\n    <br />\r\n    <button type=\"button\" id=\"logoutButton\">Logout</button>\r\n");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n</html>");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<SessionApp.Pages.User.ProtectedModel> Html { get; private set; } = default!;
        #nullable disable
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<SessionApp.Pages.User.ProtectedModel> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<SessionApp.Pages.User.ProtectedModel>)PageContext?.ViewData;
        public SessionApp.Pages.User.ProtectedModel Model => ViewData.Model;
    }
}
#pragma warning restore 1591
