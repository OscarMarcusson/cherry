# Simplified User Interface Framework
This is a simple UI language based on HTML, but built with the intention of simplifying and reducing boilerplate syntax.

#### Table of Contents
* #### UI
  * [Element Declaration](docs/syntax/Elements.md)
  * [Scope](docs/syntax/Scope.md)
  * [Styling](docs/syntax/Styles.md)
  * [Comments](docs/syntax/Comments.md)
  * [Variable Binding](docs/syntax/VariableBinding.md)
  * [Meta data](docs/syntax/Meta.md)
* #### Script
  * [Functions](docs/script/Functions.md)
  * [Variables](docs/script/Variables.md)
* #### Compiler
  * [Installation / Setup](docs/compiler/CompilerSetup.md)
  * [Arguments](docs/compiler/CompilerArguments.md)

<br>

## TL;DR
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
