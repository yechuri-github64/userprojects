package com.ai2dev.random_name.service;

import com.ai2dev.random_name.client.ExternalApiClient;
import com.ai2dev.random_name.client.SalesforceClient;
import com.ai2dev.random_name.model.User;
import com.ai2dev.random_name.model.dto.AggregatedDataDto;
import com.ai2dev.random_name.model.dto.ExternalDto;
import com.ai2dev.random_name.model.dto.SalesforceAccountDto;
import com.ai2dev.random_name.repository.UserMySqlRepository;
import jakarta.inject.Inject;
import jakarta.inject.Singleton;
import java.util.Optional;

@Singleton
public class DataService {

    @Inject
    ExternalApiClient externalApiClient;

    @Inject
    SalesforceClient salesforceClient;

    @Inject
    UserMySqlRepository userMySqlRepository;

    public AggregatedDataDto getAggregatedData(Long id) {
        // Load from MySQL
        Optional<User> userOpt = userMySqlRepository.findById(id);
        if (userOpt.isEmpty()) {
            return null;
        }
        User user = userOpt.get();

        // Fetch external API data
        ExternalDto external = null;
        try {
            external = externalApiClient.fetchData(id);
        } catch (Exception e) {
            // handle gracefully - external may be unavailable
        }

        // Fetch Salesforce account by mapping user email to account id (example)
        SalesforceAccountDto account = null;
        try {
            if (user.getEmail() != null) {
                account = salesforceClient.getAccountByEmail(user.getEmail());
            }
        } catch (Exception e) {
            // handle gracefully
        }

        AggregatedDataDto dto = new AggregatedDataDto();
        dto.setUser(user);
        dto.setExternal(external);
        dto.setSalesforceAccount(account);
        return dto;
    }
}
