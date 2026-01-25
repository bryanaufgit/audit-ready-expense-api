#!/usr/bin/env bash
set -e

echo "Starting API..."
dotnet run --project AuditReadyExpense.Api &
API_PID=$!

echo "Waiting for API..."
sleep 5

echo "API running (PID=$API_PID)"
wait $API_PID