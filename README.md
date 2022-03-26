# Simplified User Interface Framework
This is a simple UI language based on HTML, but built with the intention of simplifying and reducing boilerplate syntax.


<br>

## Syntax
The syntax is built to match HTML in terms of content, but with different syntax. 

### Element declaration
In its most simple way, you could view it as this:

**Simplified**
```ini
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

### Styling
Styling is done with standard CSS rules, although with some slight syntax modification. Instead of using the standard `{key}:{value};` we use `{key}={value}`, as well as the same indentation scoping as the elements.

**Simplified**
```ini
div
	background-color = green
	color = red
```
**CSS**
```CSS
div {
	background-color: green;
	color: red;
}
```
Applying the style to an element with the help of classes is where the differences to HTML starts to appear. In HTML we are used to adding it to the element with the help of the `<div class="{style1} {style2}"></div>`, with the CSS being named `.{name}`. In the simplified style we also add the dot in front of classes for the styles, but to apply it to an element you just append that name as is to the element name. This means we get `div.style1.style2` instead.

Note that to create a style from within the same document you have to add a root element called `style`.

**Simplified**
```ini
div.panel
	h1 = "Hello World!"
	p = "Lorem ipsum"
  
style
	body
		background-color = #141414
	.panel
		width = 500px
		margin = auto
		padding = 15px
		background-color = #1c1c1c
		color = #c7c7c7
		border-radius = 8px
```
**HTML**
```HTML
<!DOCTYPE html>
<head>
	<style>
		body {
			background-color: #141414;
		}
		.panel {
			width: 500px;
			margin: auto;
			padding: 15px;
			background-color: #1c1c1c;
			color: #c7c7c7;
			border-radius: 8px;
		}
	</style>
</head>
<body>
	<div class="panel">
		<h1>Hello World!</h1>
		<p>Lorem ipsum</p>
	</div>
</body>
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
