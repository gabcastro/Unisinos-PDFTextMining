# Busca por _strings_ em documentos

## Descrição da aplicação

Consiste em uma aplicação para localizar palavras informadas pelo usuário, em documentos PDF. 

## Como funciona?

A aplicação consite em um `console aplication` usando .net core 3.1. A mesma é constituida de duas entradas:
- Caminho do .pdf
- String de busca

A string de busca pode ser composta por uma ou mais palavras. E a entrada segue as seguintes regras:
-  As palavras que deverão ser localizadas no texto serão informadas em letras minúsculas.
-  No caso de múltiplas palavras deverá ser informado o operador lógico entre elas. Os operadores lógicos aceitos serão `AND` e `OR`.
-  Os operadores deverão ser escritos em letras maiúsculas, enquanto as palavras consultadas em letras minúsculas.

Para um resultado, existem algumas regras:
- Caso o operador `OR` seja utilizado, a aplicação retorna o número de ocorrências de cada uma das palavras consultadas.
- Caso o operador `AND` seja utilizado, a aplicação retorna se todas as palavras foram encontradas no texto, juntamente com o número de ocorrências de cada uma delas.

> Palavras iguais mas com escrita diferente (Aplicação, APLICAÇÃO, aplicacao) devem ser consideradas como uma só.

Ao final da busca, o resultado é armazenado em arquivo texto como forma de histórico das pesquisas realizadas anteriormente. O arquivo contém o número da consulta (id), o nome do documento pesquisado, a string de busca e o número de ocorrências associado com cada palavra.
