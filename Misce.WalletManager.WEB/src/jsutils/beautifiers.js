export function formatMoneyAmount(amount) {
    let amountString = amount.toString();
    amountString = amountString.replace('.', ','); //commas instead of points
    amountString = amountString.includes(',') ? amountString : amountString + ',00'; //integer numbers
    amountString = amountString.length == amountString.indexOf(',') + 3 ? amountString : amountString + '0'; //only one decimal number
    return amountString;
}