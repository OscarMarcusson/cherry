# Simplified User Interface Framework
This is a simple UI language based on HTML, but built with the intention of simplifying and reducing boilerplate syntax.


<br>

## Core syntax rules
##### Elements
```
[element_definition] = [value]

// Examples: 
p = Hello world!
button = Press me!
```
  * A value is not needed.
  * Any element name can be used but some, like "h1" or "table", already have a default behaviour. This follows the  HTML standard.


##### Scope
Scope is determined by indentation, for example:
```
some_root_element
    h1 = Scope example (I have one parent)
    some_div
        p = I have two parents!
```


<br>

## HTML comparison
Since this language is built around compilation to HTML most components can be directly translated, with only the surrounding syntax being different.

##### Simplified
```
// I am a comment!
h1 = Hello World
p = I made graphics happen, woo!
```

##### HTML
```
<!DOCTYPE html>
<html>
    <body>
        <!-- I am a comment! -->
        <h1>Hello World</h1>
        <p>I made graphics happen, woo!</p>
    </body>
</html>
```

## Setup
To run this project, install, compile with dotnet, and then run. Skip arguments or use "-h" or "--help" for more information.