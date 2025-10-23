package Testjt;

import jakarta.inject.Singleton;
import jakarta.inject.Inject;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import javax.sql.DataSource;

@Singleton
public class AccountService {
    private static final Logger LOG = LoggerFactory.getLogger(AccountService.class);

    @Inject
    private DataSource dataSource;

    public Account getLastAccount() {
        String sql = "SELECT id, name, email, created_at FROM accounts ORDER BY id DESC LIMIT 1";
        try (Connection conn = dataSource.getConnection();
             PreparedStatement ps = conn.prepareStatement(sql);
             ResultSet rs = ps.executeQuery()) {
            if (rs.next()) {
                Account a = new Account();
                a.setId(rs.getLong("id"));
                a.setName(rs.getString("name"));
                a.setEmail(rs.getString("email"));
                try {
                    a.setCreatedAt(rs.getString("created_at"));
                } catch (Exception ex) {
                    // ignore if column missing or null
                }
                return a;
            } else {
                return null;
            }
        } catch (SQLException e) {
            LOG.error("Database error while fetching last account", e);
            throw new RuntimeException("Database error: " + e.getMessage(), e);
        } catch (Exception e) {
            LOG.error("Unexpected error while fetching last account", e);
            throw new RuntimeException("Unexpected error: " + e.getMessage(), e);
        }
    }
}
