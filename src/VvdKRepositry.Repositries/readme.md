
1. BaseTablePersistence
    - inheriting class should inject TableClient
    - used for shared tables

2. UserTablePersistence
    - inheriting class should inject IAzureClientFactory. Client will be based on IdProvider.ServiceIdentifier
    - TableUrl/Account will be taken from IdProvider.TableUrl
    - Specific table will be taken from IIdProvider.Containername