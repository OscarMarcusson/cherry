# Element declaration
In its most simple way, elements are declared as `{element}={value}`, or just `{element}` for empty or hierarchical elements. The value is typically text, but can also be things like links or paths to resources. The most common would be a string value for text, like the following:

**Simplified**
```ini
div
p = "Hello World"
```
**HTML**
```HTML
<div></div>
<p>Hello World</p>
```



<br>

## String literal & variables
Implementation progress:
- [x] String literals
- [ ] Number literals
- [ ] Literal formulas like `p = 1 + 2` or `p = "test " + 512 + b`
- [x] Constant variable values 
- [x] Constant variable for string interpolation
- [ ] Dynamic variable values
- [ ] Dynamic variable for string interpolation
- [ ] Dynamic variable formulas

In the example above we used a string literal, telling the compiler to insert any text between the quotation marks. While this works for some elements we often have need of a fully dynamic value or to insert a dynamic value into a specific section of static text. This can be done by referencing a variable directly or by using string interpolation.

```ini
// By creating a string literal constant the variable content will be inserted as is during compile time
// If we use "var" instead we can update the variable in runtime and the text will change too
let some_variable = "Hello World!"

body
	p = "I'm a string literal and will show up just like this!"
	p = some_variable
	p = "Here we print the content of some_variable: {some_variable}"
	p = "   Example   with spaces   "
```

**HTML**
```HTML
<!-- Boilerplate HTML until we reach the body -->
<p>I'm a string literal and will show up just like this!</p>
<p>Hello World!</p>
<p>Here we print the content of some_variable: Hello World!</p>
<p>&nbsp;&nbsp;&nbsp;Example&nbsp;&nbsp;&nbsp;with spaces&nbsp;&nbsp;&nbsp;</p>
```

Note that string interpolation is always applied for {}, if you wish to skip this use a backslash for the opening bracket. `p = "test\{a}"` will for example produce `<p>test{a}</p>`.




<br>

## Variable Binding
By adding a bind section it's possible to sync the UI value to a variable. This is similar to the examples above, but binding is used for two-way reading where changes in the UI will change the variable and vice versa. 
#### Example
```ini
textarea bind(description_variable) = "This text can be edited"
```
For more details, see [Variable Binding](docs/syntax/VariableBinding.md)




<br>

## Extended element parsing
Anything that is not a pure text value is defined in the left section of the element declaration. The default is of course the element name, which in many cases also define some type of behaviour like **p**, **h1**, **table** and so on.

### Style

For more details, see [Styling](docs/syntax/Styles.md).


<br>

### Type
For more details, see [Type](docs/syntax/Types.md).


<br>

## Images
Declaring an image element is done via `img` or `image`, with the value being the path to the image. For example:
```ini
img = "some_directory/some_image.png"
```
### Image configurations
Keyword | Description
---|---
alt | Provides alternative information if the image can't be displayed
width | Sets the pixel width to reserve before loading the image
height | Sets the pixel height to reserve before loading the image
size | Sets both the width and height to the given value, or sets width to the first value and the height to the second value is two comma separated values are given

A full example of a typical image would be:
```ini
img.style alt("Some image") size(150,80) = "some_directory/some_image.png"
```
