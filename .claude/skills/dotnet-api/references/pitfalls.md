# Known pitfalls (P1–P10)

Used by `/dotnet-api audit` (whole codebase) and flagged by `check`. Do not copy these into new code. Delete a line once the project has fixed it.

- **P1** Ownership/authorization check that compares an id with itself (always passes), or trusts a client-supplied user id instead of the caller's claims. Nothing reads the caller's identity from `User` claims.
- **P2** Expiry unit bugs: `AddMinutes(RefreshTokenExpiryInDays)` treats days as minutes.
- **P3** No logout / token revocation endpoint.
- **P4** `[Required]` on enums or value types does nothing; a missing enum silently becomes 0 (e.g. Electronics). Make it nullable (`CATEGORY?`) + `[Required]`, or add a defined-value check.
- **P5** Copy-pasted `ErrorMessage` text that doesn't match the rule (wrong field name, wrong limits, e.g. a Password rule saying "Email is required" or "between 10 and 50" when the rule is 8–16; Gender saying "Invalid Category Provided").
- **P6** DTO limits differing from DB `HasMaxLength` (DTO 20 vs DB 120; Description 100 vs 500). Align them.
- **P7** Inconsistent DTO property naming (`Access_token`/`Refresh_token` vs `AccessToken`/`RefreshToken`; `Total_items`, `Delivery_address`). Use PascalCase.
- **P8** Response fields declared but never set (e.g. `UpdateUserResponseDto.Gender`); stale `// TODO` comments for work already done; file name not matching class name (`UpdateUserDto.cs` containing `UpdateUserRequestDto`).
- **P9** `public readonly` fields in services (use `private readonly`); `$"..."` interpolated log calls; duplicated entity→DTO mapping across `Create`/`GetAll`/`GetById`; redundant `if(!ModelState.IsValid)` in a controller.
- **P10** Dead null-checks on options already validated at startup; `ConflictException` never used where duplicates are possible.
