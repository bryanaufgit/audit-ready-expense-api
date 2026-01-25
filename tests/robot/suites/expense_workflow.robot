*** Settings ***
Library    RequestsLibrary

*** Variables ***
${BASE_URL}    http://localhost:5059
${USER_ID}     11111111-1111-1111-1111-111111111111

*** Test Cases ***
Create Draft Expense
    ${headers}=    Create Dictionary
    ...    Content-Type=application/json
    ...    X-Actor-UserId=${USER_ID}

    Create Session    api    ${BASE_URL}    headers=${headers}

    ${payload}=    Create Dictionary
    ...    title=Taxi
    ...    amount=42.50

    ${response}=    POST On Session
    ...    api
    ...    /api/expenses
    ...    json=${payload}

    Should Be Equal As Integers    ${response.status_code}    201


Create Draft Expense Without Header
    Create Session    api    ${BASE_URL}

    ${payload}=    Create Dictionary
    ...    title=Taxi
    ...    amount=42.50

    ${response}=    POST On Session
    ...    api
    ...    /api/expenses
    ...    json=${payload}
    ...    expected_status=400

    Should Be Equal As Integers    ${response.status_code}    400


Create Draft Expense With Invalid Payload
    ${headers}=    Create Dictionary
    ...    Content-Type=application/json
    ...    X-Actor-UserId=${USER_ID}

    Create Session    api    ${BASE_URL}    headers=${headers}

    ${payload}=    Create Dictionary
    ...    title=
    ...    amount=-5

    ${response}=    POST On Session
    ...    api
    ...    /api/expenses
    ...    json=${payload}
    ...    expected_status=400

    Should Be Equal As Integers    ${response.status_code}    400

Submit Draft Expense
    ${headers}=    Create Dictionary
    ...    Content-Type=application/json
    ...    X-Actor-UserId=${USER_ID}

    Create Session    api    ${BASE_URL}    headers=${headers}

    ${payload}=    Create Dictionary
    ...    title=Hotel
    ...    amount=120.00

    ${create}=    POST On Session
    ...    api
    ...    /api/expenses
    ...    json=${payload}

    Should Be Equal As Integers    ${create.status_code}    201

    ${expense_id}=    Set Variable    ${create.json()["id"]}

    ${submit}=    POST On Session
    ...    api
    ...    /api/expenses/${expense_id}/submit

    Should Be Equal As Integers    ${submit.status_code}    204