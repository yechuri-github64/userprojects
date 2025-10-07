package com.ai2dev.guess_name.domain.main;

import io.micronaut.data.annotation.Repository;
import io.micronaut.data.jpa.repository.JpaRepository;

@Repository
public interface NameGuessRepository extends JpaRepository<NameGuess, Long> {
}
