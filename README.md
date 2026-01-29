## Simulador-de-Futebol

O presente projeto representa a culminação das capacidades adquiridas ao longo do meu percurso académico no CINEL, em conjunto com os meus interesses pessoais na área da programação e do desenvolvimento de software. Este trabalho tem como objetivo principal aplicar os conhecimentos técnicos e metodológicos obtidos na criação de uma aplicação desktop.

O projeto consiste em permitir ao utilizador final simular a carreira de um atleta profissional de futebol, desde o início até ao momento em que o próprio utilizador decida terminá-la. 

O jogo tem início com a criação e personalização da personagem, permitindo ao jogador definir as características do seu atleta virtual. Após esta fase, o utilizador poderá escolher aquele em que pretende iniciar a sua carreira.
 
O atleta virtual será gerado com atributos iniciais aleatórios, o que introduz um elemento de imprevisibilidade e garante que cada simulação seja única. A partir daí, o jogador inicia a sua trajetória como um jovem de 18 anos, progredindo ao longo das épocas conforme as suas decisões e o seu desempenho.

O jogo baseia-se num sistema de turnos, sendo que um turno representa a passagem de um mês dentro do jogo. Durante este período, são simulados golos, assistências e outras estatísticas de desempenho, calculadas de acordo com os atributos do atleta. Paralelamente, podem ocorrer eventos aleatórios, que apresentam dilemas e situações que o jogador deve resolver, influenciando diretamente o rumo da carreira e da vida do atleta virtual.

Estes eventos abrangem diferentes aspetos da vida profissional e pessoal do jogador, tanto dentro como fora de campo, tornando cada jogabilidade dinâmica e imprevisível. Assim, o projeto procura oferecer uma experiência interativa e realista, onde as escolhas do utilizador têm impacto direto na evolução do seu personagem e no desfecho da sua carreira.

### Tecnologias usadas
- Unity (C#);
- Firebase (base de dados);

### Build jogável
A build funcional para Windows encontra-se disponível na secção **Releases** deste repositório, no ficheiro **Build-PC.zip**.

Após descomprimir:

 - Abrir a pasta extraída;

 - Executar "Simulador de futebol.exe";

A build inclui ligação funcional à base de dados para efeitos de demonstração.

### Nota sobre o firebase
Por razões de segurança, as credenciais do Firebase (API Key) não estão incluídas no código-fonte disponível neste repositório.
O ficheiro FirebaseManager encontra-se incompleto intencionalmente.

A build incluída no repositório contém a configuração necessária apenas para fins de demonstração.

### Estrutura do codigo
Dentro da pasta Ficheiros Fonte, o projeto está organizado da seguinte forma:

- **Menus**: Contem todos os ficheiros de código que envolvem o gerenciamento de menus;
   -  **Login**: Contem os ficheiros que permitem a navegação e a funcionalidade dos menus de login;
   -  **Mudança de Menus**: Contem o ficheiro que habilita a navegação entre menus dentro do jogo;
   -  **pop ups**: Contem os ficheiros que controlam os “Pop-Ups” dentro do jogo;

- **Clubes**: Contem os ficheiros relacionados com o gerenciamento de clubes, negociação com clubes e o Scriptable Object “LigasDB”;

-	**Firebase**: Todos os ficheiros relacionados com o firebase são guardados nesta pasta;

-	**Turnos**: Contem “TurnosManager”, o ficheiro que simula os turnos do jogo;

-	**Classes e Jogador**: Contem o ficheiro “Saves e Dados de cena” que contem duas classes importantes, e  a pasta “Jogador” que guarda todos os ficheiros relativos ao jogador;

-	**Eventos**: Contem os ficheiros que permitem os eventos aleatórios;

-	**Trofeus**: Contem todos os ficheiros que permitem os trofeus dentro do jogo;

### Estado do projeto
O projeto encontra-se concluído.
Alguns aspetos poderão ser melhorados futuramente (organização de código, otimizações e correção de bugs), mas o foco foi a implementação funcional das mecânicas principais.
