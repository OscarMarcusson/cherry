# Simplified User Interface Framework
This is a simple UI language based on HTML, but built with the intention of simplifying and reducing boilerplate syntax.

## Content
[Core Syntax](docs/CoreSyntax.md)
[Styling](docs/Styles.md)

<br>

### Scope
Scope is decided by newlines and tabs, similar to how you would do it in Python or other simplified scripting languages:

**Simplified**
```ini
center
	div
		p = "Some text"
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
```ini
// I am a comment!
p = "Some text"
```
**HTML**
```HTML
<!-- I am a comment! -->
<p>Some text</p>
```


<br>


## Setup
To run this project, install, compile with dotnet, and then run. Skip arguments or use "-h" or "--help" for more information.
