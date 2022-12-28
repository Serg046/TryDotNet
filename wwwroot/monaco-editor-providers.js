function registerProviders(dotnetHelper) {
    window.monaco.languages.registerCompletionItemProvider("csharp", {
        triggerCharacters: ["."],
        // this signature is outdated, actually it is resolveCompletionItem: (item, token)
        // https://github.com/microsoft/vscode/commit/ce850e02d56cd3ff5b5a93ce23c2272d3bac0fa2
        resolveCompletionItem: (model, position, item) => {
            return this.resolveCompletionItem(item, dotnetHelper)
        },
        provideCompletionItems: (model, position, context) => {
            return this.provideCompletionItems(model, position, context, dotnetHelper)
        }
    });
}

let _lastCompletions;

async function provideCompletionItems(model, position, context, dotnetHelper) {
    let request = this._createRequest(position);
    request.CompletionTrigger = (context.triggerKind + 1);
    request.TriggerCharacter = context.triggerCharacter;

    try {
        const code = model.getValue();
        const completions = await dotnetHelper.invokeMethodAsync("GetCompletionAsync", code, request);
        return { suggestions: completions };
    }
    catch (error) {
        return;
    }
}

async function resolveCompletionItem(item, dotnetHelper) {
    const lastCompletions = this._lastCompletions;
    if (!lastCompletions) {
        return item;
    }

    const lspItem = lastCompletions.get(item);
    if (!lspItem) {
        return item;
    }

    const request = { Item: lspItem };
    try {
        const response = await dotnetHelper.invokeMethodAsync("GetCompletionResolveAsync", request);
        return this._convertToVscodeCompletionItem(response.item);
    }
    catch (error) {
        return;
    }
}

function _createRequest(position) {

    let Line, Column;

    Line = position.lineNumber - 1;
    Column = position.column - 1;

    return {
        Line,
        Column
    };
}