
## Development notes

- The `AppHost` project is the entrypoint; it composes service registrations and references using helper extension methods.
- The `Database` project is a `.sqlproj` and may have build steps that publish schema or artifacts consumed by the Data API Builder.
- The data API builder (`dab`) reads configuration from `../api/dab-config.json`. Ensure that file exists and contains the expected configuration for endpoints.

## Troubleshooting

- Connection errors:
  - Confirm SQL Server and Redis are reachable at the host/port given in `AppHost/AppHost.cs`.
  - Verify credentials and network/firewall settings.
- Missing `dab-config.json`:
  - Create `api/dab-config.json` or change the path in `AppHost/AppHost.cs`.
- Build/runtime errors:
  - Run `dotnet build` for detailed compiler errors.
  - Inspect host logs printed to the console when running `dotnet run`.

## Contributing

Contributions are welcome. Prefer small, focused pull requests. When changing secrets or default configuration, document the change in this `README.md`.

## License

No license file included. Add a license that suits your needs (e.g., `MIT`) and document it here.
