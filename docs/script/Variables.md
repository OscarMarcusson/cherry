# Variables
A variable can be declared inside a [function](docs/script/Functions.md) body for local use, or outside to become globally accessible.

## Declaration
A variable is declared by starting a line with `var` or `let`, where `var` is a variable that can have its value updated and `let` is locked to its initial value. 
The declaration is then followed by the `type` and `name`. 
The most basic declaration would be:
```d
var int a
let int b
```
If no value is supplied the default for the given type is used. This would be `0` for numbers, `""` for strings, `false` for booleans, `null` for nullables and so on. 
The most common way of declaring a variable would be to give the variable a value from the start, which can be done like the following:
```d
var int a = 14
let int b = 5623
```
> Any type errors will be marked by the compiler, for example:
> 
> ![image](https://user-images.githubusercontent.com/62374937/161436191-78d7f4dd-623d-4796-b5d0-b11ccac13011.png)

When a value is supplied it is not necessary to define the type, since it can be resolved by the value set. The above example could be simplified to:
```d
var a = 14
let b = 5623
```


## Variable assignment
`var` variables can be updated after declaration by calling the variable name and any of the assignment operators. For example:
```d
var a = 12345
let b = 6
a = b + 4
// a is now 10
```
### Number assignment operators
| Operator | Declaration  | Operator  | Result |
| -------- | ------------ | --------- | ------ |
| +=       | var a = 5    | a += 5    | 10     |
| -=       | var a = 10   | a -= 4    | 6      |
| \*=      | var a = 4    | a \*= 2   | 8      |
| /=       | var a = 20   | a /= 5    | 4      |
| ++       | var a = 2    | a ++      | 3      |
| --       | var a = 2    | a --      | 1      |

### Boolean assignment operators
| Operator | Declaration   | Operator  | Result |
| -------- | ------------- | --------- | ------ |
| &=       | var a = true  | a &= true | true   |
| !=       | var a = false | a != true | true   |
| !!       | var a = true  | a !!      | false  |
