# Element declaration
In its most simple way, elements are declared as `{element}={value}`, or just `{element}` for empty or hierarchical elements.

**Simplified**
```ini
div
p = Hello World
```
**HTML**
```HTML
<div></div>
<p>Hello World</p>
```



<br>

## Value parsing
In the example above the value is written directly, which means it will be parsed as "everything after =", trimmed. If you wish to include spaces or tabs at the start or end of the value you have to wrap it in quotation marks, like this: 
```ini
p = "  String with spaces  "
```
**HTML**
```HTML
<p>&nbsp;&nbsp;String with spaces&nbsp;&nbsp;</p>
```



<br>

## Extended element parsing
Anything that is not a pure text value is defined in the left section of the element declaration. The default is of course the element name, which in many cases also define some type of behaviour like **p**, **h1**, **table** and so on.

### Style

For more details, see [Styling](docs/syntax/Styles.md).


<br>

### Type
For more details, see [Type](docs/syntax/Types.md).


<br>
### Variable Binding
By adding a bind section it's possible to sync the UI value to a variable. 
This works for any element, it's for example possible to bind a paragraph (p), heading (h1, h2, etc) or even a div or button. 
This will enable changing their text value on the fly by just changing the bound variable, which can be useful for displaying things like user names. 
Binding is also very useful for reading data, like getting the value of some text input, or perhaps the value of a checkbox.
#### Example
```ini
textarea bind(description_variable) = "This text can be edited"
``` to the variable declaration
For more details, see [Variable Binding](docs/syntax/VariableBinding.md)