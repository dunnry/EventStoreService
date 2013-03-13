EventStoreService
=================

This is a TopShelf wrapper around the EventStore (http://geteventstore.com) so you can host this as a service.


Sample Configuration

    <?xml version="1.0" encoding="utf-8" ?>
    <configuration>
      <configSections>
        <section name="eventStore" type="EventStoreService.EventStoreServiceConfiguration, EventStoreService, Version=1.0.0.0, Culture=neutral" />
      </configSections>
       
      <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" />
      </startup>
     
       <eventStore>
        <instance name="Production" dbPath="f:\eventstoredb" filePath="f:\eventstore\EventStore.SingleNode.exe" cachedChunkCount="1" tcpPort="5001" httpPort="5501" runProjections="false"/>
      </eventStore>
      
    </configuration>

If the following settings are optional and will default:

* httpPort - defaults to 2113
* tcpPort - defaults to 1113
* runProjections - defaults to false
