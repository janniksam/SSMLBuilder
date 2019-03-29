# SSMLBuilder
Yet another SSML Builder, that uses the Razor Engine to render the SSML output

[![Build status master](https://ci.appveyor.com/api/projects/status/ncsfmp2cixpgc9m3?svg=true&passingText=master%20-%20passing&failingText=master%20-%20failing&pendingText=master%20-%20pending)](https://ci.appveyor.com/project/janniksam/SSMLVerifier) 
[![Build status dev](https://ci.appveyor.com/api/projects/status/ncsfmp2cixpgc9m3/branch/dev?svg=true&passingText=dev%20-%20passing&failingText=dev%20-%20failing&pendingText=dev%20-%20pending)](https://ci.appveyor.com/project/janniksam/SSMLVerifier/branch/dev)
[![NuGet version](https://badge.fury.io/nu/SSMLBuilder.svg)](https://badge.fury.io/nu/SSMLBuilder)

## Why did I create this builder?

Personally, I don't like coding down stuff in an imperative manner, when it comes to reports or other similar UI'ish stuff. Microsoft offers the Razor engine to develop powerful web-frontends with a mix of HTML and C#. The best thing about razor is that it is not restricted to the use inside an ASP.NET application.

Since SSML is more or less XML with a fixed subset of tags, I figured, why not try to build my SSML Builder using the powerful Razor Engine

## How does it work?

Basically, you first have to build a .cshtml-View we know from ASP.NET application. That view will later be compiled into the SSML.

Here is a simple example:

```xml
@using RazorEngine.Templating
@inherits TemplateBase<dynamic>
<speak>
    <p>
        The clock is ticking down
        5<break time='0.3s' />
        4<break time='0.3s' />
        3<break time='0.3s' />
        2<break time='0.3s' />
        1<break time='0.8s' />

        @if (Model.PlayerAmount == 5 || Model.PlayerAmount == 6)
        {
            <p>
                We started with less than 7 people.
                <break time='5s' />
                3<break time='0.3s' />
                2<break time='0.3s' />
                1<break time='0.3s' />
            </p>
        }
        else
        {
            <p>
                We started with more than 6 people.
                <break time='5s'/>
                3<break time='0.3s'/>
                2<break time='0.3s'/>
                1<break time='0.3s'/>
            </p>
        }
        
        This is the end
    </p>
</speak>
```

When you finished, you can throw the view into the builder, append a model and finally let the razor engine do it's magic:

```cs
var templateKey = "SSMLBuilderTests.SSMLViews.TestView.cshtml";
var assembly = GetType().GetTypeInfo().Assembly;
var resource = assembly.GetManifestResourceStream(templateKey);
var ssmlResult = await SSMLRazorBuilder.BuildFromAsync(resource, templateKey, new { PlayerAmount = 5 });
```  

The result in this example will look like this:

```xml
<speak>
    <p>
        The clock is ticking down
        5<break time='0.3s' />
        4<break time='0.3s' />
        3<break time='0.3s' />
        2<break time='0.3s' />
        1<break time='0.8s' />

            <p>
                We started with less than 7 people.
                <break time='5s' />
                3<break time='0.3s' />
                2<break time='0.3s' />
                1<break time='0.3s' />
            </p>
        
        This is the end
    </p>
</speak>
```  

I used a dynamic model in this example. Feel free whatever type you like.

## Verification

Be sure to always verify the SSML after the razor engine, since I cannot guarantee that your razor view is SSML-conform.

You can find my SSMLVerifier [here](https://github.com/janniksam/SSMLVerifier).
