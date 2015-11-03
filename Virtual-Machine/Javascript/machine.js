/*
 * Devoir 1 - Machine virtuelle pour l'architecture Little Man Computer
 *
 * Auteur:      Christophe Savard
 * Matricule:   20034327
 */

// Utilisez ce message pour les entrées de la machine.
var MESSAGE_INPUT = "Entrez une valeur numérique entre -500 et 499.";

//Objet regroupant plusieurs fonctions utilitaires
var Utils =
{
    /*
     * S'assure que lors de l'addition ou de la soustraction, le résultat reste parmis
     * les nombres compris par la machine virtuelle (-500 à 499)
     *
     * param{Number} n: Nombre à vérifier
     * return{Number}: Résultat restreint de -500 à 499
     */
    Clamp: function (n)
    {
        //Au dessus de 499 deviens négatif (499 + 1 = -500)
        if (n > 499) { return n - 1000; }
        //Sous -500 deviens positif (-500 - 1 = 499)
        if (n < -500) { return n + 1000; }

        return n;   //Si parmi les bornes, ne pas modifier
    },

    /*
     * Prends un nombre entre 0 et 999 sans signe et le transforme en un nombre
     * avec signe entre -500 et 499
     *
     * param{Number} n: Nombre à transformer en nombre avec signe
     * return{Number}: Résultat de la conversion
     */
    ToSigned: function (n)
    {
        //Tous les nombres au dessus de 499 deviennent négatifs
        if (n > 499) { return n - 1000; }

        return n;   //Si sous 500, ne pas modifier
    },

    /*
     * Transform un nombre signé entre -500 et 499 et le transform en nombre
     * non signé entre 0 et 999
     * 
     * param{Number} n: Nombre à transformer en nombre non signé
     * return{Number}: Résultat de la conversion
     */
    ToUnsigned: function (n)
    {
        //Tous les nombres négatifs deviennent plus grand que 499
        if (n < 0) { return n + 1000; }

        return n;   //Sinon, ne pas modifier
    },

    /*
     * Vérifie si une suite de charactères est une étiquette (finie par ':')
     *
     * param{String} s: Chaine de charactère à vérifier
     * return{Bool}: Si la chaine de charactère finie par ':' ou non
     */
    IsLabel: function (s) 
    {
        return s[s.length - 1] === ':';
    },

    /*
     * Enlève le dernier charactère d'un string et retourne le résultat
     *
     * param{String} s: Chaine de charachtère à modifier
     * return{String}: Chaine de charactère sans son dernier charactère
     */
    GetLabel: function (s)
    {
        return s.substring(0, s.length - 1);
    }
};

//Objet regroupant toutes les commands possibles de la machine LMC
var instructions =
{
    "ADD": 100,     //Addition              1XX
    "SUB": 200,     //Soustraction          2XX
    "STA": 300,     //Sauvegarde            3XX
    "LDA": 500,     //Chargement            5XX
    "BRA": 600,     //Banche                6XX
    "BRZ": 700,     //Branche si zéro       7XX
    "BRP": 800,     //Branche si positif    8XX
    "INP": 901,     //Entrée                901
    "OUT": 902,     //Sortie                902
    "HLT": 0,       //Fin                   000
    "DAT": 0        //Data                  XXX
};


/*
 * Retourne un enregistrement représentant l'état d'une machine LMC. 
 * 
 * param{Number[]} mem: La mémoire de la machine virtuelle
 * param{Number} pc: Le pointeur d'instruction 
 * param{Number} acc: L'accumulateur
 * return{LMCState}: Un enregistrement représentant l'état d'une machine LMC.
 */
function LMCState(mem, pc, acc)
{
    return { mem: mem, pc: pc, acc: acc };
}

/*
 * La fonction 'compile' fait la transformation de
 * code source en une série d'instructions pour l'architecture
 * Little Man Computer.
 * 
 * Example:
 * $> compile("lda A\nsub B\nout\nA: dat 90\nB: dat 80");
 *  > 10
 *
 * param{String} source: Le code source
 * return{Number[]}: Une série d'instructions prêtes à être exécutées.
 */
function compile(source)
{
    var lines = SplitLines(source);     //Sépare les lignes
    var delabeled = Delabel(lines);     //Désétiquette le code
    var code = Generate(delabeled);     //Génère les instructions
    return code;
}

/*
 * Reçoit la mémoire avec laquelle procéder à l'exécution.
 *
 * param{Number[]} mem: La mémoire 
 * return{LMCState}: Un enregistrement représentant l'état final de la machine.
 */
function execute(mem)
{
    var state = LMCState(mem, 0, 0);    //Initialise l'état de la machine LMC

    //Continue temps que le pointeur n'est pas à la fin de la mémoire
    while (state.pc < state.mem.length)
    {
        var c = state.mem[state.pc];    //Instruction courrante

        /*
         * Vérifie si le programme doit halter et retourne l'état de la machine si c'est le cas
         *      c < 100         -> Une instruction commencant par zéro signifie la fin du programme
         * c>= 400 && c < 500   -> Une instruction commencant par quatre n'existe pas
         *      c > 902         -> Une instruction plus grande que 902 n'existe pas
         */
        if (c < 100 || (c >= 400 && c < 500) || c > 902) { return state; }

        state.pc++;                             //On incrémente le pointeur d'instruction
        state = ExecuteInstruction(c, state);   //Éxécute l'instruction courrante
    }

    //Lorsque terminé, retourner l'état de la machine
    return state;
}

/*
 * Prends le code source de la machine et le transforme en un tableau de tableaux string
 * contenant les lignes de code et les instructions séparées
 *
 * param{String} source: Le code source
 * return{String[][]}: Le tableaux des lignes étant des tableaux de chaque instruction
 */
function SplitLines(source)
{
    var lines = source.toUpperCase().split("\n");       //Mets le code en majuscules et sépare les lignes au charactère de saut de ligne
    var result = [];                                    //Initialise le tableau résultat

    //Boucle à travers toutes les lignes
    for (var i = 0; i < lines.length; i++)
    {
        var c = SplitInstructions(lines[i]);    //Sépare les lignes en instructions

        //Si la ligne n'est pas vide, ajoute le tableau aux résultats
        if (c != null) { result.push(c); }
    }

    //Retourne le tableau résultat
    return result;
}

/*
 * Prends une ligne de code source et la sépare en instructions individuelles
 *
 * param{String} line: La ligne de code à séparer
 * return{String[]}: La ligne séparée en instructions
 */
function SplitInstructions(line)
{
    //Si la ligne est vide, retourner null
    if (line == "") { return null; }

    //Sépare chaque mot aux espaces
    return line.split(' ');
}

/*
 * Prends les lignes d'instructions séparées et désétiquettes tout le code
 * en remplacent les étiquettes pour le numéro de ligne en question
 *
 * param{String[][]} lines: Code séparé en lignes et en instructions
 * return{string[][]}: Code désétiquetté et séparé
 */
function Delabel(lines)
{
    var labels = {};    //Initialisation de l'enregistrement d'étiquettes

    //Boucle à travers toutes les lignes
    for (var i = 0; i < lines.length; i++)
    {
        var line = lines[i];    //Ligne courrante
        var label = line[0];    //Premier élément de la ligne

        //Vérifie si le premier élément de la ligne est une étiquette
        if (Utils.IsLabel(label))
        {
            labels[Utils.GetLabel(label)] = i;      //Ajoute l'étiquette à l'enregistrement
            lines[i] = line.slice(1);               //Enlève l'étiquette de la ligne et la remplace dans le tableau
        }
    }

    //Boucle à travers toutes les lignes
    for (var i = 0; i < lines.length; i++)
    {
        var line = lines[i];    //Ligne courrante

        //Vérifie que la ligne à deux éléments (instructions + nombre/étiquette)
        if (line.length == 2)
        {
            var label = line[1];    //Deuxième élément de la ligne

            //Vérifie que le deuxième élément n'est pas un chiffre
            if (isNaN(parseInt(label)))
            {
                line[1] = labels[label].toString();     //Remplace l'étiquette par le chiffre correspondant
            }
        }
    }

    return lines;
}

/*
 * Prends le code désétiquetté et le transformme en instructions numériques mémoire
 * qui peuvent être exécuté par la machine virtuelle LMC
 *
 * param{String[][]} delabeled: Le code séparé et désétiquetté
 * return{Number[]}: Les instructions mémoire pouvant être exécutées
 */
function Generate(delabeled)
{
    var code = [];      //Initialise le tableau résultat

    //Boucle à travers toutes les lignes
    for (var i = 0; i < delabeled.length; i++)
    {
        var line = delabeled[i];    //Ligne courrante
        var index = 0;              //Initialise l'index de ligne

        //Si la ligne à deux éléments, obtenir l'index à partir du deuxième élément
        if (line.length == 2) { index = parseInt(line[1]); }

        //Ajouter le nombre relier à l'instruction + l'index au tableau de code
        code.push(instructions[line[0]] + index);
    }

    //Retourner les instructions
    return code;
}

/*
 * Éxécute une instruction spécifique du programme LMC
 *
 * param{Number} c: Instruction à éxécuter
 * parame{LMCState} state: État actuel de la machine LMC
 * return{LMCState}: L'état de la machine après l'éxécution de l'instruction
 */
function ExecuteInstruction(c, state)
{
    var index = c % 100;                    //Obtiens l'index de position
    var instruction = (c - index) / 100;    //Obtiens le numéro d'instruction

    //Décide de l'instruction à éxécuter
    switch (instruction)
    {
        //ADD - 1XX
        case 1:
            {
                /*
                 * 1. Fait l'addition signée de l'accumulateur et du nombre à l'endroit indiqué par l'index
                 * 2. S'assure que le résultat est de -500 à 499
                 * 3. Transforme le résultat en nombre non signé de 0 à 999 et le mets dans l'accumulateur
                 */
                state.acc = Utils.ToUnsigned(Utils.Clamp(Utils.ToSigned(state.acc) + Utils.ToSigned(state.mem[index])));
                break;
            }

            //SUB - 2XX
        case 2:
            {
                /*
                 * 1. Fait la soustraction signée de l'accumulateur et du nombre à l'endroit indiqué par l'index
                 * 2. S'assure que le résultat est de -500 à 499
                 * 3. Transforme le résultat en nombre non signé de 0 à 999 et le mets dans l'accumulateur
                 */
                state.acc = Utils.ToUnsigned(Utils.Clamp(Utils.ToSigned(state.acc) - Utils.ToSigned(state.mem[index])));
                break;
            }

            //STA - 3XX
        case 3:
            {
                state.mem[index] = state.acc;   //Met le nombre dans l'accumulateur dans la mémoire à l'index donné
                break;
            }

            //LDA - 5XX
        case 5:
            {
                state.acc = state.mem[index];   //Met le nombre dans la mémoire à l'index donné dans l'accumulateur
                break;
            }

            //BRA - 6XX
        case 6:
            {
                state.pc = index;               //Rempalce le pointeur d'instruction pour l'index
                break;
            }

            //BRZ - 7XX
        case 7:
            {
                //Remplace le pointeur d'instruction pour l'index si l'accumulateur est égal à zéro
                if (state.acc == 0) { state.pc = index; }
                break;
            }

            //BRP - 8XX
        case 8:
            {
                //Remplace le pointeur d'instruction pour l'index si l'accumulateur est positif
                if (state.acc > 0 && state.acc <= 499) { state.pc = index; }
                break;
            }

            //INP/OUT - 901/902
        case 9:
            {
                //INP - 901
                if (index == 1)
                {
                    //Obtiens un nombre entre -500 et 499 pars prompt
                    var n = parseInt(prompt(MESSAGE_INPUT, ""));

                    //Vérifie si le nombre est valide
                    while (n < -500 || n > 499 || isNaN(n))
                    {
                        //Sinon, demande un nombre valide à nouveau, jusqu'a ce qu'il valide
                        n = parseInt(prompt("Nombre invalide. " + MESSAGE_INPUT, ""));
                    }

                    state.acc = Utils.ToUnsigned(n);      //Mets le nombre dans l'accumulateur sous forme non signée
                    break;
                }

                //OUT - 902
                if (index == 2)
                {
                    //Imprime le nombre dans l'accumulateur sous forme signée
                    print("Sortie: " + Utils.ToSigned(state.acc));
                    break;
                }
            }
    }

    //On retourn l'état final de la machine
    return state;
}

/*
 * ==================================================================
 *  Exemples de programmes
 * ==================================================================
 */

/*
 * Affiche la différence absolue de deux nombres
 * i.e.  |a-b|
 *
 * Ligne    Programme    Code machine
 * =====    =========    ============
 * 000      INP          901     Saisit un nombre
 * 001      STA a        310     Enregistre le nombre saisie
 * 002      INP          901     Saisit un deuxième nombre
 * 003      SUB a        210     Soustrait le premier au deuxième
 * 004      BRP print    808     Si positif imprimer
 * 005      STA a        310     Sinon enregistre la valeur
 * 006      SUB a        210     Et la soustrait 2 fois
 * 007      SUB a        210     pour obtenir sa valeur absolue
 * 008      print: OUT   902     IMPRIME!
 * 009      HLT          000
 * 010      a: DAT 0     000
 */

var DIFFERENCE = "INP\n"
               + "STA a\n"
               + "INP\n"
               + "SUB a\n"
               + "BRP PRINT\n"
               + "STA a\n"
               + "SUB a\n"
               + "SUB a\n"
               + "PRINT: OUT\n"
               + "HLT\n"
               + "a: DAT 0";
