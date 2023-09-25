# rinha

Tree-Walking Interpreter implemented in dotnet.

## Como executar

First build the container:

```sh
cd rinha
docker build . -t="rinha"
```

Finally execute:

```sh
docker run -it rinha fileName ./files//fib.json
```

## Language Features

``` 
pensando em meios de rodar por escopos, retornos dinamicos C# :D
```

- [x] Function  
- [x] Let  
- [x] Call
- [x] Var
- [x] Int
- [x] Str
- [x] Binary
- [x] If
- [x] Print
- [x] Bool
- [x] First
- [x] Second
- [x] Tuple
