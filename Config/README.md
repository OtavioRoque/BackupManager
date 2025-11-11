# Configuração de Connection String

Esta pasta contém os arquivos de configuração da connection string usados pelo projeto.

## 🧩 `App.config`

- Versionado no repositório..
- Contém uma connection string fictícia, usada apenas como modelo de referência.

## 🔒 `App.Local.config`

- Não versionado (está no `.gitignore`).
- Deve conter sua connection string real para acesso ao SQL Server.
- Para criar:
  - Faça uma cópia de `App.config`.
  - Renomeie a cópia para `App.Local.config`.
  - Edite o valor do atributo `connectionString` com seus dados reais.
