# Styling
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

## Hover & Click
It's possible to add interactive styling by putting it under a section called **hover** or **click**:

**Simplified**
```ini
div
	background-color = red
	hover
		background-color = green
	click
		background-color = blue
```
**CSS**
```CSS
div {
	background-color: red;
}
div:hover {
	background-color: green;
}
div:active {
	background-color: blue;
}
```

<br>

## Media queries
Media queries like checking for viewport size can be done by simply writing an if-statement in the middle of the style:

**Simplified**
```ini
div
	background-color = red
	if width >= 700px
		background-color = green
```
**CSS**
```CSS
div {
	background-color: red;
}
@media (min-width:700px) {
	div {
		background-color: green;
	}
}
```
It is also possible to add the **if** statement at the root level which will produce the same result but allows multiple elements to be placed under it.
