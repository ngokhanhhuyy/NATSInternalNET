using System.Text.RegularExpressions;

namespace NATSInternal.Services;

public partial class SqlExceptionHandler
{
    public void Handle(MySqlException exception)
    {
        Match match;
        switch (exception.Number)
        {
            case 1062:
                IsUniqueConstraintViolated = true;
                match = UniqueConstraintRegex().Match(exception.Message);
                if (match.Success)
                {
                    ViolatedTableName = match.Groups["tableName"].Value;
                    ViolatedConstraintName = match.Groups["constraintName"].Value;
                    ViolatedFieldName = ViolatedConstraintName.Split("__").Last();
                    ViolatedValue = match.Groups["duplicatedKeyValue"].Value;
                }

                break;

            case 1364:
                IsNotNullConstraintViolated = true;
                match = NotNullConstraintRegex().Match(exception.Message);
                if (match.Success)
                {
                    ViolatedFieldName = match.Groups["columnName"].Value;
                }

                break;

            case 1406:
                IsMaxLengthExceeded = true;
                match = MaxLengthConstraintRegex().Match(exception.Message);
                if (match.Success)
                {
                    ViolatedFieldName = match.Groups["columnName"].Value;
                }

                break;

            case 1451:
                IsDeleteOrUpdateRestricted = true;
                match = DeleteOrUpdateRestrictedRegex().Match(exception.Message);
                if (match.Success)
                {
                    ViolatedConstraintName = match.Groups["constraintName"].Value;
                    ViolatedTableName = match.Groups["tableName"].Value;
                    ViolatedFieldName = match.Groups["columnName"].Value;
                }

                break;

            case 1452:
                IsForeignKeyNotFound = true;
                match = ForeignKeyNotFoundRegex().Match(exception.Message);
                if (match.Success)
                {
                    ViolatedConstraintName = match.Groups["constraintName"].Value;
                    ViolatedTableName = match.Groups["tableName"].Value;
                    ViolatedFieldName = match.Groups["columnName"].Value;
                }

                break;
        }
    }

    public bool IsUniqueConstraintViolated { get; protected set; }

    public bool IsNotNullConstraintViolated { get; protected set; }

    public bool IsMaxLengthExceeded { get; protected set; }

    public bool IsForeignKeyNotFound { get; protected set; }

    public bool IsDeleteOrUpdateRestricted { get; protected set; }

    public string ViolatedTableName { get; protected set; }

    public string ViolatedFieldName { get; protected set; }

    public string ViolatedConstraintName { get; protected set; }

    public object ViolatedValue { get; protected set; }

    [GeneratedRegex(@"Duplicate entry\s+\'(?<duplicatedKeyValue>.+)\'\s+for key\s+\'(?<tableName>\w+)\.(?<constraintName>\w+)'")]
    private static partial Regex UniqueConstraintRegex();

    [GeneratedRegex(@"Field\s+\'(?<columnName>.+)\'\s+doesn't have a default value")]
    private static partial Regex NotNullConstraintRegex();

    [GeneratedRegex(@"Data truncation: Data too long for column '(?<columnName>.+)' at row (?<rowNumber>.+)")]
    private static partial Regex MaxLengthConstraintRegex();

    [GeneratedRegex(@"(?<databaseName>.+?)\.`(?<tableName>.+)`, CONSTRAINT `(?<constraintName>.+)` FOREIGN KEY \(`(?<columnName>.+)`\) REFERENCES `(?<referencedTableName>.+)` \(`(?<referencedColumnName>.+)`\)")]
    private static partial Regex ForeignKeyNotFoundRegex();

    [GeneratedRegex(@"Cannot delete or update a parent row: a foreign key constraint fails \(`(?<databaseName>.+)`\.`(?<tableName>.+)`, CONSTRAINT `(?<constraintName>.+)` FOREIGN KEY \(`(?<columnName>.+)`\) REFERENCES `(?<referenceTableName>.+)`\)")]
    private static partial Regex DeleteOrUpdateRestrictedRegex();
}
