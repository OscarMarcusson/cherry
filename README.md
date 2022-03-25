# Simplified User Interface Framework
This is a simple UI language based on HTML, but built with the intention of simplifying and reducing boilerplate syntax.


<br>

## Syntax
The syntax is built to match HTML in terms of content, but with different syntax. 

### Element declaration
In its most simple way, you could view it as this:

**Simplified**
```C#
div
p = Hello World
p = "String version of Hello World, if spaces should be included at the start or end like this  "
```
**HTML**
```HTML
<div></div>
<p>Hello World</p>
<p>String version of Hello World, if spaces should be included at the start or end like this&nbsp;&nbsp;</p>
```
<br>

### Scope
Scope is decided by newlines and tabs, similar to how you would do it in Python or other simplified scripting languages:

**Simplified**
```C#
center
  div
    p = Some text
```
**HTML**
```HTML
<center>
  <div>
    <p>Some text</p>
  </div>
</center>
```
<br>

### Comments
Comments are written as with most languages, with the double slashes //.

**Simplified**
```C#
// I am a comment!
p = Some text
```
**HTML**
```HTML
<!-- I am a comment! -->
<p>Some text</p>
```


<br>


## Setup
To run this project, install, compile with dotnet, and then run. Skip arguments or use "-h" or "--help" for more information.
