function registerMonacoProviders(roslynService) {
    window.monaco.languages.registerCompletionItemProvider("csharp", {
        triggerCharacters: ["."],
        provideCompletionItems: async (model, position, context) => {
            const code = model.getValue();
            const request = {
                Line: position.lineNumber - 1,
                Column: position.column - 1
            };

            return {
                suggestions: await roslynService.invokeMethodAsync("GetCompletions", code, request)
            };
        }
    });
}