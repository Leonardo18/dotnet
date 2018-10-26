module BrazilianUtils.Cnpj

open Helpers
open System.Text

let private cnpjLength = 14
let private firstCheckDigitWeights = [ 5; 4; 3; 2; 9; 8; 7; 6; 5; 4; 3; 2 ]
let private secondCheckDigitWeights = [ 6; 5; 4; 3; 2; 9; 8; 7; 6; 5; 4; 3; 2 ]

let private digitRule digit =
    match digit with
    | 0 | 1 -> 0
    | _ -> 11 - digit

let private calculateDigit weights value =
    value
    |> calculateModulus11 weights
    |> digitRule

let private validateCheckDigit value weights checkDigit =
    value
    |> calculateDigit weights
    |> ((=) checkDigit)

let private isValidFirstCheckDigit (value : int list) =
    let checkDigitPos = 12
    let checkDigit = value.[checkDigitPos]
    validateCheckDigit value firstCheckDigitWeights checkDigit

let private isValidSecondCheckDigit (value : int list) =
    let checkDigitPos = 13
    let checkDigit = value.[checkDigitPos]
    validateCheckDigit value secondCheckDigitWeights checkDigit

let private hasCnpjLength = hasLength cnpjLength

//  Visible members
let IsValid cnpj =
    let clearValue = stringToIntList cnpj
    [ hasValue; hasCnpjLength; isNotRepdigit; isValidFirstCheckDigit; isValidSecondCheckDigit ]
    |> List.forall (fun validator -> validator clearValue)

let Format cnpj =
    let clearValue = OnlyNumbers cnpj
    StringBuilder(clearValue).Insert(2, ".").Insert(6, ".").Insert(10, "/").Insert(15, "-").ToString()

let Generate () =
    let baseCnpj = generateRandomNumbers (cnpjLength - 2)
    let firstCheckDigit = calculateDigit firstCheckDigitWeights baseCnpj
    let secondCheckDigit = calculateDigit secondCheckDigitWeights (baseCnpj@[firstCheckDigit])
    baseCnpj @ [firstCheckDigit] @ [secondCheckDigit]
    |> List.map (fun x -> x.ToString())
    |> String.concat ""
